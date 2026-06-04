using AutoMapper;
using AutoMapper.QueryableExtensions;
using BulkyWeb.Exceptions;
using BulkyWeb.Models;
using BulkyWeb.Models.Caches;
using BulkyWeb.Models.DTO.Category;
using BulkyWeb.Models.DTO.Category.Elastic;
using BulkyWeb.Repository.IRepository;
using BulkyWeb.Services.Caches;
using BulkyWeb.Validators.Categories;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Clients.Elasticsearch.QueryDsl;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;


namespace BulkyWeb.Services.Categories
{
    public class CategoryService : ICategoryService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;
        private readonly IMemoryCache _cache;
        private readonly ICacheService _cacheService;
        private readonly ElasticsearchClient _elasticClient;

        private const string IndexName = "categories";
        private const int BatchSize = 1000;
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CategoryService> logger, IMemoryCache cache
            , ICacheService CacheService, ElasticsearchClient elasticClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
            _cacheService = CacheService;
            _elasticClient = elasticClient;
        }



        public async Task<CategorySearchResultDTO> SearchAsync(CategoryFilterDTO filter, CancellationToken ct = default)
        {
            var fromEs = await TrySearchCategoriesElasticAsync(filter, ct);
            if (fromEs is not null)
                return fromEs;

            // fallback به SQL (از GetAllAsync استفاده می‌کنیم)
            var items = await GetAllAsync(filter);

            return new CategorySearchResultDTO
            {
                Items = items,
                TotalCount = items.Count // ساده: چون GetAllAsync الان count جدا نمی‌دهد
            };
        }







        private async Task<CategorySearchResultDTO?> TrySearchCategoriesElasticAsync(CategoryFilterDTO filter, CancellationToken ct = default)
        {
            int page = filter.PageNumber is > 0 ? filter.PageNumber : 1;
            int pageSize = filter.PageSize is > 0 ? filter.PageSize : 10;

            var exists = await _elasticClient.Indices.ExistsAsync(IndexName, ct);
            if (!exists.IsValidResponse || !exists.Exists)
            {
                _logger.LogDebug("Elastic index '{IndexName}' not ready or not found.", IndexName);
                return null;
            }

            var mustQueries = new List<Query>();

            if (filter.CategoryID.HasValue)
            {
                mustQueries.Add(new Query
                {
                    Term = new TermQuery { Field = "categoryID", Value = filter.CategoryID.Value }
                });
            }

            if (!string.IsNullOrWhiteSpace(filter.CategoryName))
            {
                mustQueries.Add(new Query
                {
                    Match = new MatchQuery { Field = "categoryName", Query = filter.CategoryName }
                });
            }

            var response = await _elasticClient.SearchAsync<CategoryElasticDocument>(s => s
                .Indices(IndexName)
                .From((page - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q.Bool(b => b.Must(mustQueries)))
                .Sort(so =>
                {
                    if (filter.SortBy == "id_desc")
                        so.Field("categoryID", fs => fs.Order(SortOrder.Desc));
                    else if (filter.SortBy == "name_asc")
                        so.Field("categoryName.keyword", fs => fs.Order(SortOrder.Asc));
                    else if (filter.SortBy == "name_desc")
                        so.Field("categoryName.keyword", fs => fs.Order(SortOrder.Desc));
                    else
                        so.Field("categoryID", fs => fs.Order(SortOrder.Asc));
                })
            , ct);

            // اگر ES مشکل داشت، اینجا fallback کن (نه exception)
            if (!response.IsValidResponse)
            {
                _logger.LogWarning("Elastic search failed, falling back to SQL. Debug: {Debug}", response.DebugInformation);
                return null;
            }

            return new CategorySearchResultDTO
            {
                Items = response.Documents.Select(x => new CategoryGetAllDTO
                {
                    CategoryID = x.categoryID,
                    CategoryName = x.categoryName,
                    Description = x.description,
                    Picture = x.picture
                }).ToList(),
                TotalCount = response.HitsMetadata?.Total?.Value1?.Value ?? 0
            };
        }





        public async Task<List<CategoryGetAllDTO>> GetAllAsync(CategoryFilterDTO filterDTO)
        {
            var page = filterDTO.PageNumber > 0 ? filterDTO.PageNumber : 1;
            var size = filterDTO.PageSize > 0 ? filterDTO.PageSize : 10;
            var cacheKey = CacheKeys.Categories(filterDTO);
            var result = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var query = _unitOfWork.CategoryRepository.Query(); // IQueryable<Category>

                //if (query =)
                //{
                //    throw new NotFoundException("No categories found");

                //}


                if (filterDTO.CategoryID.HasValue)
                    query = query.Where(x => x.CategoryID == filterDTO.CategoryID);

                if (!string.IsNullOrWhiteSpace(filterDTO.CategoryName))
                    query = query.Where(x => x.CategoryName.Contains(filterDTO.CategoryName));

                // Sorting
                switch (filterDTO.SortBy)
                {
                    case "id_desc":
                        query = query.OrderByDescending(x => x.CategoryID);
                        break;

                    case "name_asc":
                        query = query.OrderBy(x => x.CategoryName);
                        break;

                    case "name_desc":
                        query = query.OrderByDescending(x => x.CategoryName);
                        break;

                    default:
                        query = query.OrderBy(x => x.CategoryID); // default
                        break;
                }

                // Pagination
                query = query
                    .Skip((page - 1) * size)
                    .Take(size);

                return await _mapper
                    .ProjectTo<CategoryGetAllDTO>(query)
                    .ToListAsync();
            });



            return result;


        }



        public async Task<CategoryGetByIdDTO> GetByIdAsync(int id)
        {
            if (id <= 0) { throw new ValidationException("Id is not valid"); }

            var fromEs = await GetCategoryByIdElasticAsync(id);
            if (fromEs != null)
            {
                return fromEs;
            }
            var fromSql = await GetCategoryByIdSqlAsync(id);
            if (fromSql is null)
                throw new NotFoundException("This Id is not recognized");

            return fromSql;
            //var result = await _unitOfWork.CategoryRepository
            //.Query()
            //.Where(x => x.CategoryID == id)
            //.ProjectTo<CategoryGetByIdDTO>(_mapper.ConfigurationProvider)
            //.FirstOrDefaultAsync();

            //if (result == null)
            //{
            //    throw new NotFoundException("This Id is not recognized");
            //}

            //return result;
        }
        private async Task<CategoryGetByIdDTO?> GetCategoryByIdSqlAsync(int id)
        {
            return await _unitOfWork.CategoryRepository
                .Query()
                .Where(x => x.CategoryID == id)
                .ProjectTo<CategoryGetByIdDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }



        public async Task<CategoryResponseDTO> CategoryCreate(CategoryCreateDTO filter)
        {

            var validator = new CategoryCreateValidator();
            var result = validator.Validate(filter);

            if (!result.IsValid)
            {
                _logger.LogWarning("Validation failed for {@CategoryData}", new { filter.CategoryName, filter.Description });
                _logger.LogWarning("Second Validation failed for {@CategoryData}", filter);
                _logger.LogWarning("thrid Validation failed for {@Filter} ", filter);

                throw new ValidationException(result.Errors);
            }
            var entity = _mapper.Map<Category>(filter);

            try
            {
                await _unitOfWork.CategoryRepository.AddAsync(entity);
                await _unitOfWork.SaveAsync();

                try
                {
                    await IndexCategoryAsync(entity);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to index created category {CategoryId} in elastic", entity.CategoryID);

                }

                //await IndexCategoryAsync(entity);
                CacheKeys.CategoryVersion++;
                return new CategoryResponseDTO
                {
                    Message = "Category Created in Service"
                };
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Request was cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during category creation");
                throw new DbUpdateException("Database error", ex);
            }


        }



        public async Task<CategoryResponseDTO> CategoryUpdateServiece(int id, CategoryUpdateDTO obj)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid ID parameter");
            }
            if (obj == null)

            {
                throw new ValidationException("Not Full Entity or wrong");

            }
            var entity = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

            if (entity == null)
                throw new NotFoundException("Not Founded this Category");

            var validator = new CategoryUpdateValidator();
            var result = validator.Validate(obj);


            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }

            _mapper.Map(obj, entity);

            try
            {

                await _unitOfWork.SaveAsync();
                try
                {
                    await IndexCategoryAsync(entity);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to index Update category {CategoryId} in elastic", entity.CategoryID);

                }
                CacheKeys.CategoryVersion++;

                return new CategoryResponseDTO
                {
                    Message = "Category Updated in Service"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during category update");
                throw new DbUpdateException("Database error during update", ex);
            }

        }


        private async Task IndexCategoryAsync(Category entity, CancellationToken ct = default)
        {
            var doc = new CategoryElasticDocument
            {
                categoryID = entity.CategoryID,
                categoryName = entity.CategoryName,
                description = entity.Description,
                picture = entity.Picture
            };

            var response = await _elasticClient.IndexAsync(doc, i => i
                .Index(IndexName)
                .Id(entity.CategoryID.ToString()), ct);

            if (!response.IsValidResponse)
                throw new Exception(response.DebugInformation);
        }

        private async Task DeleteCategoryFromIndexAsync(int id, CancellationToken ct = default)
        {
            var response = await _elasticClient.DeleteAsync<CategoryElasticDocument>(id.ToString(), d => d.Index(IndexName), ct);

            if (!response.IsValidResponse)
                throw new Exception(response.DebugInformation);
        }




        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            if (id <= 0)
            {
                throw new ValidationException("Id is invalid");
            }

            var category = await _unitOfWork.CategoryRepository.GetAsync(u => u.CategoryID == id);

            if (category == null)
            {
                throw new NotFoundException("Category  Not Founded ");
            }
            _unitOfWork.CategoryRepository.Remove(category);


            await _unitOfWork.SaveAsync();
            try { await DeleteCategoryFromIndexAsync(id, ct); }
            catch (Exception ex) { _logger.LogError(ex, "Delet is not done in RemoveCategory"); }

            CacheKeys.CategoryVersion++;
        }



















        //Elastics

        //public async Task<object> SyncCategoriesToElastic()
        //{
        //    var categories = await _unitOfWork.CategoryRepository.Query().ToListAsync();
        //    Console.WriteLine($"تعداد محصولاتی که از categories اومد: {categories.Count}");

        //    var categoryDtos = _mapper.Map<List<CategoryGetAllDTO>>(categories);
        //    Console.WriteLine($"تعداد دسته‌بندی که از دیتابیس اومد: {categoryDtos.Count}");

        //    var hasDuplicateIds = categoryDtos.GroupBy(x => x.CategoryID).Any(g => g.Count() > 1);
        //    Console.WriteLine($"آیا آیدی تکراری داریم؟ {hasDuplicateIds}");

        //    var existsResponse = await _elasticClient.Indices.ExistsAsync("categories");
        //    if (!existsResponse.Exists)
        //    {
        //        await _elasticClient.Indices.CreateAsync("categories");
        //    }


        //    //var response = await _elasticClient.Index("categories").IndexManyAsync(categoryDtos, "categories");
        //    var response = await _elasticClient.BulkAsync(b => b
        //        .Index("categories")
        //        .IndexMany(categoryDtos, (descriptor, s) => descriptor.Id(s.CategoryID)) //  
        //    );

        //    //var response = await _elasticClient.Indices.CreateAsync("categories", c => c
        //    //    .Mappings(m => m
        //    //        .Properties(p => p
        //    //            .Number(n => n
        //    //                .Name("categoryName")
        //    //                .Type(NumberType.Integer)
        //    //            )
        //    //        )
        //    //    )
        //    //);
        //    if (categoryDtos.Count > 0)
        //    {
        //        categoryDtos.Add(categoryDtos[0]); // duplicate id
        //        Console.WriteLine("یک رکورد تکراری عمداً اضافه شد.");
        //    }
        //    if (response.Errors)
        //    {
        //        var failed = response.Items
        //            .Where(i => i.Error is not null)
        //            .Select(i => new { i.Id, i.Status, i.Error })
        //            .ToList();

        //        Console.WriteLine($"Bulk partial failures: {failed.Count}");
        //        foreach (var f in failed.Take(20))
        //            Console.WriteLine($"FAILED id={f.Id} status={f.Status} error={f.Error}");

        //        throw new Exception("Bulk had item-level errors. Check logs.");
        //    }

        //    if (!response.IsValidResponse)
        //    {
        //        Console.WriteLine("خطا در Bulk: " + response.DebugInformation);
        //        throw new Exception(response.DebugInformation);
        //    }
        //    else
        //    {
        //        Console.WriteLine($"تعداد عملیات موفق: {response.Items.Count}");
        //    }

        //    return new { dbCount = categories.Count, elasticCount = categoryDtos.Count };

        //}


        public async Task<CategorySyncResult> SyncCategoriesToElastic(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Category sync started.");

            //  await EnsureIndexExistsAsync(cancellationToken);

            var dbCount = await _unitOfWork.CategoryRepository.Query()
                .AsNoTracking()
                .CountAsync(cancellationToken);

            _logger.LogInformation("Total categories in database: {Count}", dbCount);

            int page = 0;
            int totalSynced = 0;

            while (true)
            {
                var chunk = await _unitOfWork.CategoryRepository.Query()
                    .AsNoTracking()
                    .OrderBy(x => x.CategoryID)
                    .Skip(page * BatchSize)
                    .Take(BatchSize)
                    .ToListAsync(cancellationToken);

                if (chunk.Count == 0)
                    break;

                //var dtos = _mapper.Map<List<CategoryElasticDocument>>(chunk);
                var dtos = chunk.Select(c => new CategoryElasticDocument
                {
                    categoryID = c.CategoryID,
                    categoryName = c.CategoryName,
                    description = c.Description,
                    picture = c.Picture
                }).ToList();



                // (Optional) detect duplicates in THIS CHUNK
                var dupIds = dtos.GroupBy(x => x.categoryID).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                if (dupIds.Count > 0)
                    _logger.LogWarning("Duplicate CategoryIDs found in chunk: {Ids}", dupIds);

                // Build bulk request (index ops with explicit _id)
                var bulkRequest = new BulkRequest(IndexName)
                {
                    Operations = new BulkOperationsCollection()
                };

                foreach (var dto in dtos)
                {
                    bulkRequest.Operations.Add(new BulkIndexOperation<CategoryElasticDocument>(dto)
                    {
                        Id = dto.categoryID.ToString()
                    });
                }

                //var bulkResponse = await _elasticClient.BulkAsync(bulkRequest, cancellationToken);
                var bulkResponse = await _elasticClient.BulkAsync(b => b
    .Index(IndexName)
    .IndexMany(dtos, (descriptor, doc) => descriptor
        .Id(doc.categoryID)
    ), cancellationToken);



                if (!bulkResponse.IsValidResponse)
                {
                    _logger.LogError("Bulk request failed. Debug: {Debug}", bulkResponse.DebugInformation);
                    throw new Exception(bulkResponse.DebugInformation);
                }

                // Item-level errors
                if (bulkResponse.Errors == true)
                {
                    // Items is a collection of response items; each item may contain Error
                    var failed = bulkResponse.Items?
                        .Where(i => i.Error is not null)
                        .Select(i => new
                        {
                            i.Id,
                            i.Status,
                            ErrorType = i.Error!.Type,
                            ErrorReason = i.Error!.Reason
                        })
                        .Take(20)
                        .ToList();

                    if (failed is not null)
                    {
                        foreach (var f in failed)
                            _logger.LogError("Bulk item failed. Id={Id}, Status={Status}, Type={Type}, Reason={Reason}",
                                f.Id, f.Status, f.ErrorType, f.ErrorReason);
                    }

                    throw new Exception("Bulk had item-level errors. Check logs.");
                }

                totalSynced += dtos.Count;

                _logger.LogInformation("Chunk synced. Page={Page}, ChunkSize={Size}, TotalSynced={Total}",
                    page, dtos.Count, totalSynced);

                page++;
            }

            // Count in Elasticsearch
            var countResponse = await _elasticClient.CountAsync<CategoryElasticDocument>(c => c.Indices(IndexName), cancellationToken);
            if (!countResponse.IsValidResponse)
            {
                _logger.LogError("Count request failed. Debug: {Debug}", countResponse.DebugInformation);
                throw new Exception("Count request failed.");
            }

            var result = new CategorySyncResult
            {
                DbCount = dbCount,
                SyncedCount = totalSynced,
                ElasticCount = countResponse.Count
            };

            _logger.LogInformation("Category sync completed. Db={Db}, Synced={Synced}, ElasticCount={Elastic}",
                result.DbCount, result.SyncedCount, result.ElasticCount);

            return result;
        }

        private async Task EnsureIndexExistsAsync(CancellationToken cancellationToken)
        {
            var exists = await _elasticClient.Indices.ExistsAsync(IndexName);

            if (!exists.Exists)
            {

                var response = await _elasticClient.Indices.CreateAsync(IndexName, cancellationToken);

                if (!response.IsValidResponse)
                {
                    _logger.LogError("Index creation failed: {Debug}", response.DebugInformation);
                    throw new Exception("Index creation failed.");
                }
            }
        }
        //private async Task EnsureIndexExistsAsync(CancellationToken cancellationToken)
        //{
        //    var exists = await _elasticClient.Indices.ExistsAsync(IndexName, cancellationToken);
        //    if (!exists.IsValidResponse)
        //        throw new Exception(exists.DebugInformation);

        //    if (exists.Exists)
        //    {
        //        _logger.LogInformation("Index '{Index}' already exists.", IndexName);
        //        return;
        //    }

        //    var request = new CreateIndexRequest(IndexName)
        //    {
        //        Mappings = new TypeMapping
        //        {
        //            Properties = new Properties
        //    {
        //        { "categoryID", new IntegerNumberProperty() },
        //        {
        //            "categoryName",
        //            new TextProperty
        //            {
        //                Analyzer = "standard",
        //                Fields = new Properties
        //                {
        //                    { "keyword", new KeywordProperty { IgnoreAbove = 256 } }
        //                }
        //            }
        //        }
        //    }
        //        }
        //    };

        //    var create = await _elasticClient.Indices.CreateAsync(request, cancellationToken);

        //    if (!create.IsValidResponse)
        //    {
        //        _logger.LogError("Failed to create index '{Index}'. Debug: {Debug}", IndexName, create.DebugInformation);
        //        throw new Exception($"Could not create index '{IndexName}'.");
        //    }

        //    _logger.LogInformation("Index '{Index}' created successfully.", IndexName);
        //}

        //private async Task EnsureIndexExistsAsync(CancellationToken cancellationToken)
        //{
        //    var exists = await _elasticClient.Indices.ExistsAsync(IndexName, cancellationToken);
        //    if (!exists.IsValidResponse)
        //    {
        //        _logger.LogError("Index exists check failed. Debug: {Debug}", exists.DebugInformation);
        //        throw new Exception("Index exists check failed.");
        //    }

        //    if (exists.Exists)
        //    {
        //        _logger.LogInformation("Index '{Index}' already exists.", IndexName);
        //        return;
        //    }

        //    // Create index with explicit mapping
        //    var create = await _elasticClient.Indices.CreateAsync(IndexName, c => c
        //        .Mappings(m => m
        //            .Properties(ps => ps
        //                .Number(n => n
        //                    .Name("categoryID")      // ensure matches JSON field names you send
        //                    .Type(NumberType.Integer)
        //                )
        //                .Text(t => t
        //                    .Name("categoryName")
        //                    .Analyzer("standard")
        //                    .Fields(f => f
        //                        .Keyword(k => k
        //                            .Name("keyword")
        //                            .IgnoreAbove(256)
        //                        )
        //                    )
        //                )
        //            )
        //        ),
        //        cancellationToken);

        //    if (!create.IsValidResponse)
        //    {
        //        _logger.LogError("Failed to create index '{Index}'. Debug: {Debug}", IndexName, create.DebugInformation);
        //        throw new Exception($"Could not create index '{IndexName}'.");
        //    }

        //    _logger.LogInformation("Index '{Index}' created successfully.", IndexName);
        //}















































        //public async Task<CategorySearchResultDTO> ListSearchCategoriesElasticAsync(CategoryFilterDTO filter)
        //{
        //    int page = filter.PageNumber is > 0 ? filter.PageNumber : 1;
        //    int pageSize = filter.PageSize is > 0 ? filter.PageSize : 10;

        //    // تعریف لیست به صورت List<Query>
        //    var mustQueries = new List<Query>();

        //    if (filter.CategoryID.HasValue)
        //    {
        //        mustQueries.Add(new Query
        //        {
        //            Term = new TermQuery { Field = "categoryID", Value = filter.CategoryID.Value }
        //        });
        //    }

        //    if (!string.IsNullOrWhiteSpace(filter.CategoryName))
        //    {
        //        mustQueries.Add(new Query
        //        {
        //            Match = new MatchQuery { Field = "categoryName", Query = filter.CategoryName }
        //        });
        //    }



        //    var response = await _elasticClient.SearchAsync<CategoryElasticDocument>(s => s
        //        .Indices(IndexName)
        //        .From((page - 1) * pageSize)
        //        .Size(pageSize)
        //        .Query(q => q.Bool(b => b.Must(mustQueries)))
        //        .Sort(so =>
        //        {
        //            if (filter.SortBy == "id_desc")
        //                so.Field("categoryID", fs => fs.Order(SortOrder.Desc));
        //            else if (filter.SortBy == "name_asc")
        //                so.Field("categoryName.keyword", fs => fs.Order(SortOrder.Asc));
        //            else if (filter.SortBy == "name_desc")
        //                so.Field("categoryName.keyword", fs => fs.Order(SortOrder.Desc));
        //            else
        //                so.Field("categoryID", fs => fs.Order(SortOrder.Asc));
        //        })
        //    );

        //    if (!response.IsValidResponse)
        //        throw new Exception(response.DebugInformation);

        //    return new CategorySearchResultDTO
        //    {
        //        Items = response.Documents.Select(x => new CategoryGetAllDTO
        //        {
        //            CategoryID = x.categoryID,
        //            CategoryName = x.categoryName,
        //            Description = x.description,
        //            Picture = x.picture
        //        }).ToList(),
        //        TotalCount = response.HitsMetadata?.Total?.Value1?.Value ?? 0
        //        //Items = response.Documents.ToList(),
        //        //TotalCount = response.HitsMetadata?.Total?.Value1?.Value ?? 0
        //    };
        //}





        //private async Task<bool> EnsureIndexExistsSafeAsync(CancellationToken ct)
        //{
        //    var exists = await _elasticClient.Indices.ExistsAsync(IndexName, ct);
        //    if (!exists.IsValidResponse) return false;

        //    if (exists.Exists) return true;

        //    await EnsureIndexExistsAsync(ct);
        //    return true;
        //}




        private async Task<CategoryGetByIdDTO?> GetCategoryByIdElasticAsync(int categoryId)
        {
            if (categoryId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(categoryId));
            }
            var exists = await _elasticClient.Indices.ExistsAsync(IndexName);
            if (!exists.IsValidResponse || !exists.Exists)
            {

                //await EnsureIndexExistsAsync(CancellationToken.None);
                _logger.LogDebug("Elastic index '{IndexName}' not ready or not found. ", IndexName);
                return null;
            }

            var response = await _elasticClient.GetAsync<CategoryElasticDocument>(categoryId.ToString(), g => g
        .Index(IndexName)
    );

            if (!response.IsValidResponse || !response.Found || response.Source is null)
                return null;



            return new CategoryGetByIdDTO
            {
                CategoryID = response.Source.categoryID,
                CategoryName = response.Source.categoryName,
                Description = response.Source.description,
                Picture = response.Source.picture

                //response.Found ? response.Source : null;
            };
        }





    }


}
