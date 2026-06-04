using AutoMapper;
using AutoMapper.QueryableExtensions;
using BulkyWeb.Exceptions;
using BulkyWeb.Models.Caches;
using BulkyWeb.Models.DTO.Product;
using BulkyWeb.Models.DTO.Product.ProductElasticDocument;
using BulkyWeb.Repository.IRepository;
using BulkyWeb.Services.Caches;
using BulkyWeb.Validators.Products;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Clients.Elasticsearch.QueryDsl;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProductSyncResult = BulkyWeb.Models.DTO.Product.ProductSyncResult;
namespace BulkyWeb.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;
        private readonly IMemoryCache _cache;
        private readonly ICacheService _CacheService;
        private readonly ElasticsearchClient _client;

        private const string IndexName = "products";
        private const int BatchSize = 1000;

        public ProductService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ProductService> logger,
            IMemoryCache cache,
            ICacheService CacheService,
            ElasticsearchClient client
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
            _CacheService = CacheService;
            _client = client;
        }

        //https://localhost:7210/product/GetAll?CategoryID=10
        //https://localhost:7210/product/GetAll?ProductName=Iph
        //https://localhost:7210/product/GetAll?ProductName=a
        //    https://localhost:7210/product/GetAll?MinUnitPrice=900&MaxUnitPrice=980
        //    https://localhost:7210/product/GetAll?SortBy=name_desc


        //        public async Task<object> SyncProductsToElastic()
        //        {
        //            var products = await _unitOfWork.ProductRepository.Query().Include(p => p.Category).ToListAsync();
        //            Console.WriteLine($"تعداد محصولاتی که از products اومد: {products.Count}");

        //            var productDtos = _mapper.Map<List<ProductGetAllDTO>>(products);
        //            Console.WriteLine($"تعداد محصولاتی که از دیتابیس اومد: {productDtos.Count}");

        //            var hasDuplicateIds = productDtos.GroupBy(x => x.ProductID).Any(g => g.Count() > 1);
        //            Console.WriteLine($"آیا آیدی تکراری داریم؟ {hasDuplicateIds}");

        //            var existsResponse = await _client.Indices.ExistsAsync("products");
        //            if (!existsResponse.Exists)
        //            {
        //                await _client.Indices.CreateAsync("products");
        //            }


        //            //var response = await _client.Index("products").IndexManyAsync(productDtos, "products");
        //            var response = await _client.BulkAsync(b => b
        //    .Index("products")
        //    .IndexMany(productDtos, (descriptor, s) => descriptor.Id(s.ProductID)) //  
        //);
        //            if (!response.IsValidResponse)
        //            {
        //                Console.WriteLine("خطا در Bulk: " + response.DebugInformation);
        //                throw new Exception(response.DebugInformation);
        //            }
        //            else
        //            {
        //                Console.WriteLine($"تعداد عملیات موفق: {response.Items.Count}");
        //            }

        //            return new { dbCount = products.Count, elasticCount = productDtos.Count };




        //        }


        public async Task<ProductSearchResultDTO> SearchProductsElasticAsync(ProductFilterDTO filter)
        {
            int page = filter.PageNumber is > 0 ? filter.PageNumber.Value : 1;
            int pageSize = filter.PageSize is > 0 ? filter.PageSize.Value : 10;

            // تعریف لیست به صورت List<Query>
            var mustQueries = new List<Query>();

            if (filter.ProductID.HasValue)
            {
                mustQueries.Add(new Query
                {
                    Term = new TermQuery { Field = "productID", Value = filter.ProductID.Value }
                });
            }

            if (!string.IsNullOrWhiteSpace(filter.ProductName))
            {
                mustQueries.Add(new Query
                {
                    Match = new MatchQuery { Field = "productName", Query = filter.ProductName }
                });
            }

            if (filter.CategoryID.HasValue)
            {
                mustQueries.Add(new Query
                {
                    Term = new TermQuery { Field = "categoryID", Value = filter.CategoryID.Value }
                });
            }

            if (filter.MinUnitPrice.HasValue || filter.MaxUnitPrice.HasValue)
            {
                mustQueries.Add(new Query
                {
                    Range = new NumberRangeQuery
                    {
                        Field = "unitPrice",
                        Gte = filter.MinUnitPrice.HasValue ? (double?)filter.MinUnitPrice.Value : null,
                        Lte = filter.MaxUnitPrice.HasValue ? (double?)filter.MaxUnitPrice.Value : null
                    }
                });
            }

            var response = await _client.SearchAsync<ProductGetAllDTO>(s => s
                .Indices("products")
                .From((page - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q.Bool(b => b.Must(mustQueries))) // اینجا لیست رو پاس میدیم
                .Sort(so =>
                {
                    if (filter.SortBy == "id_desc")
                        so.Field("productID", fs => fs.Order(SortOrder.Desc));
                    else if (filter.SortBy == "name_asc")
                        so.Field("productName.keyword", fs => fs.Order(SortOrder.Asc));
                    else if (filter.SortBy == "name_desc")
                        so.Field("productName.keyword", fs => fs.Order(SortOrder.Desc));
                    else
                        so.Field("productID", fs => fs.Order(SortOrder.Asc));
                })
            );

            if (!response.IsValidResponse)
                throw new Exception(response.DebugInformation);

            return new ProductSearchResultDTO
            {
                Items = response.Documents.ToList(),
                TotalCount = response.HitsMetadata?.Total?.Value1?.Value ?? 0
            };
        }





        //    public async Task<ProductGetByIdDTO?> GetProductByIdElasticAsync(int productId)
        //    {

        //        if (productId <= 0)
        //        {
        //            throw new ArgumentNullException(nameof(productId));
        //        }

        //        var response = await _client.GetAsync<ProductGetByIdDTO>(productId, g => g
        //    .Index("products")
        //);

        //        if (!response.IsValidResponse)
        //            throw new Exception(response.DebugInformation);

        //        return response.Found ? response.Source : null;

        //    }





































        public async Task<object> GetProductsElasticStatusAsync()
        {
            var indexName = "products";

            var existsResp = await _client.Indices.ExistsAsync(indexName);
            if (!existsResp.Exists)
                return new { indexExists = false, esCount = 0, dbCount = await _unitOfWork.ProductRepository.Query().CountAsync() };

            var esCountResp = await _client.CountAsync(c =>
            {
                c.Indices(indexName);
            });
            if (!esCountResp.IsValidResponse)
                throw new Exception(esCountResp.DebugInformation);

            var dbCount = await _unitOfWork.ProductRepository.Query().CountAsync();

            return new
            {
                indexExists = true,
                esCount = esCountResp.Count,
                dbCount = dbCount,
                isEmpty = esCountResp.Count == 0,
                countsMatch = esCountResp.Count == dbCount
            };
        }





























        public async Task<ProductGetByIdDTO> GetByIdAsync(int id)
        {
            var file = await _unitOfWork.ProductRepository.Query()
                .AsNoTracking()
                .Include(c => c.Category)
                .Where(x => x.ProductID == id)
                .ProjectTo<ProductGetByIdDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (file == null)
                throw new NotFoundException("Product Not Found");

            return file;
        }
        //public async Task<ProductGetByIdDTO> GetByIdAsync(int id)
        //{
        //    var file = await _unitOfWork.ProductRepository.Query()
        //        .Where(x => x.ProductID == id)
        //        .ProjectTo<ProductGetByIdDTO>(_mapper.ConfigurationProvider)
        //        .FirstOrDefaultAsync();

        //    if (file == null)
        //        throw new NotFoundException("Product Not Found");

        //    return file;
        //}
        public async Task<ProductResponseDTO> ProductCreate(ProductCreateDTO obj)
        {
            if (obj == null)
                throw new ArgumentException("Arguments aren't correct");

            var validator = new ProductCreateValidator();
            var res = validator.Validate(obj);

            if (!res.IsValid)
                throw new ValidationException(res.Errors);

            var query = _mapper.Map<Product>(obj);

            try
            {
                await _unitOfWork.ProductRepository.AddAsync(query);
                await _unitOfWork.SaveAsync();
                CacheKeys.ProductVersion++;

                return new ProductResponseDTO
                {
                    Message = "Product Created in Service"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during order creation");
                throw new DbUpdateException("Database error", ex);
            }
        }

        public async Task<ProductResponseDTO> ProductUpdateService(int id, ProductUpdateDTO obj)
        {
            if (obj is null)
                throw new ArgumentException("model is not valid");

            var validator = new ProductUpdateValidator();
            var res = validator.Validate(obj);

            if (!res.IsValid)
                throw new ValidationException(res.Errors);

            var entity = await _unitOfWork.ProductRepository.GetByIdAsync(id);

            if (entity == null)
                throw new NotFoundException("Not Founded here");

            _mapper.Map(obj, entity);

            await _unitOfWork.SaveAsync();
            CacheKeys.ProductVersion++;

            return new ProductResponseDTO
            {
                Message = "Product Updated in Service"
            };
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Id Not valid");

            var Product = await _unitOfWork.ProductRepository.GetByIdAsync(id);

            if (Product == null)
                throw new NotFoundException("ID is not correct");

            _unitOfWork.ProductRepository.Remove(Product);

            await _unitOfWork.SaveAsync();
            CacheKeys.ProductVersion++;
        }
















        public async Task<ProductSearchResultDTO> SearchAsync(ProductFilterDTO filter, CancellationToken ct = default)
        {
            var fromEs = await TrySearchProductsElasticAsync(filter, ct);
            if (fromEs is not null)
                return fromEs;

            // fallback به SQL
            var items = await GetAllAsync(filter);

            return new ProductSearchResultDTO
            {
                Items = items,
                TotalCount = items.Count // ساده (مثل CategoryService). اگر TotalCount دقیق می‌خوای پایین می‌گم چطور.
            };
        }
        private async Task<ProductSearchResultDTO?> TrySearchProductsElasticAsync(ProductFilterDTO filter, CancellationToken ct = default)
        {
            int page = filter.PageNumber is > 0 ? filter.PageNumber.Value : 1;
            int pageSize = filter.PageSize is > 0 ? filter.PageSize.Value : 10;

            var exists = await _client.Indices.ExistsAsync(IndexName, ct);
            if (!exists.IsValidResponse || !exists.Exists)
            {
                _logger.LogDebug("Elastic index '{IndexName}' not ready or not found.", IndexName);
                return null;
            }

            var mustQueries = new List<Query>();

            if (filter.ProductID.HasValue)
                mustQueries.Add(new Query { Term = new TermQuery { Field = "productID", Value = filter.ProductID.Value } });

            if (!string.IsNullOrWhiteSpace(filter.ProductName))
                mustQueries.Add(new Query { Match = new MatchQuery { Field = "productName", Query = filter.ProductName } });

            if (filter.CategoryID.HasValue)
                mustQueries.Add(new Query { Term = new TermQuery { Field = "categoryID", Value = filter.CategoryID.Value } });

            if (filter.MinUnitPrice.HasValue || filter.MaxUnitPrice.HasValue)
            {
                mustQueries.Add(new Query
                {
                    Range = new NumberRangeQuery
                    {
                        Field = "unitPrice",
                        Gte = filter.MinUnitPrice.HasValue ? (double?)filter.MinUnitPrice.Value : null,
                        Lte = filter.MaxUnitPrice.HasValue ? (double?)filter.MaxUnitPrice.Value : null
                    }
                });
            }

            if (filter.MinUnitsInStock.HasValue || filter.MaxUnitsInStock.HasValue)
            {
                mustQueries.Add(new Query
                {
                    Range = new NumberRangeQuery
                    {
                        Field = "unitsInStock",
                        Gte = filter.MinUnitsInStock.HasValue ? (double?)filter.MinUnitsInStock.Value : null,
                        Lte = filter.MaxUnitsInStock.HasValue ? (double?)filter.MaxUnitsInStock.Value : null
                    }
                });
            }

            var response = await _client.SearchAsync<ProductElasticDocument>(s => s
                .Indices(IndexName)
                .From((page - 1) * pageSize)
                .Size(pageSize)
                //.TrackTotalHits(true)
                .Query(q => q.Bool(b => b.Must(mustQueries)))
                .Sort(so =>
                {
                    if (filter.SortBy == "id_desc")
                        so.Field("productID", fs => fs.Order(SortOrder.Desc));
                    else if (filter.SortBy == "name_asc")
                        so.Field("productName.keyword", fs => fs.Order(SortOrder.Asc));
                    else if (filter.SortBy == "name_desc")
                        so.Field("productName.keyword", fs => fs.Order(SortOrder.Desc));
                    else
                        so.Field("productID", fs => fs.Order(SortOrder.Asc));
                }), ct);

            if (!response.IsValidResponse)
            {
                _logger.LogWarning("Elastic search failed, falling back to SQL. Debug: {Debug}",
                    response.DebugInformation);
                return null;
            }

            // تبدیل ElasticDocument -> DTO
            var items = response.Documents.Select(d => new ProductGetAllDTO
            {
                ProductID = d.productID,
                ProductName = d.productName,
                CategoryID = d.categoryID,
                CategoryName = d.categoryName,
                QuantityPerUnit = d.quantityPerUnit,
                UnitPrice = d.unitPrice,
                UnitsInStock = d.unitsInStock,
                UnitsOnOrder = d.unitsOnOrder,
                ReorderLevel = d.reorderLevel,
                Discontinued = d.discontinued
            }).ToList();

            return new ProductSearchResultDTO
            {
                Items = items,
                TotalCount = response.HitsMetadata?.Total?.Value1?.Value ?? 0
            };
        }





        public async Task<ProductGetByIdDTO> GetByIdAsync(int id, CancellationToken ct = default)
        {
            if (id <= 0) throw new ValidationException("Id is not valid");

            var fromEs = await GetProductByIdElasticAsync(id, ct);
            if (fromEs is not null)
                return fromEs;

            var fromSql = await GetProductByIdSqlAsync(id, ct);
            if (fromSql is null)
                throw new NotFoundException("Product Not Found");

            return fromSql;
        }

        private async Task<ProductGetByIdDTO?> GetProductByIdSqlAsync(int id, CancellationToken ct)
        {
            // اگر DTO شما CategoryName لازم دارد، Include/Projection را طوری تنظیم کن
            return await _unitOfWork.ProductRepository.Query()
                .AsNoTracking()
                .Include(c => c.Category)
                .Where(x => x.ProductID == id)
                .ProjectTo<ProductGetByIdDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);
        }

        private async Task<ProductGetByIdDTO?> GetProductByIdElasticAsync(int id, CancellationToken ct)
        {
            var exists = await _client.Indices.ExistsAsync(IndexName, ct);
            if (!exists.IsValidResponse || !exists.Exists)
            {
                _logger.LogDebug("Elastic index '{IndexName}' not ready or not found.", IndexName);
                return null;
            }

            var response = await _client.GetAsync<ProductElasticDocument>(id.ToString(), g => g.Index(IndexName), ct);

            if (!response.IsValidResponse || !response.Found || response.Source is null)
                return null;

            var s = response.Source;

            // map to DTO (دقیقاً مطابق ProductGetByIdDTO خودت تنظیم کن)
            return new ProductGetByIdDTO
            {
                ProductID = s.productID,
                ProductName = s.productName,
                CategoryName = s.categoryName,
                UnitPrice = s.unitPrice,
                UnitsInStock = s.unitsInStock,
                QuantityPerUnit = s.quantityPerUnit,
                UnitsOnOrder = s.unitsOnOrder,
                ReorderLevel = s.reorderLevel,
                Discontinued = s.discontinued
            };
        }


        public async Task<List<ProductGetAllDTO>?> GetAllAsync(ProductFilterDTO filter)
        {
            var page = filter.PageNumber is > 0 ? filter.PageNumber.Value : 1;
            var size = filter.PageSize is > 0 ? filter.PageSize.Value : 10;

            var cacheKey = CacheKeys.Products(filter);

            var result = await _CacheService.GetOrCreateAsync(cacheKey, async () =>
           {
               //var query = _unitOfWork.ProductRepository.Query().AsNoTracking();
               var query = _unitOfWork.ProductRepository.Query();

               if (filter.ProductID.HasValue)
                   query = query.Where(x => x.ProductID == filter.ProductID);

               if (!string.IsNullOrWhiteSpace(filter.ProductName))
                   query = query.Where(x => x.ProductName.Contains(filter.ProductName));

               if (filter.CategoryID.HasValue)
                   query = query.Where(x => x.CategoryID == filter.CategoryID);

               if (filter.MinUnitPrice.HasValue)
                   query = query.Where(x => x.UnitPrice >= filter.MinUnitPrice);

               if (filter.MaxUnitPrice.HasValue)
                   query = query.Where(x => x.UnitPrice <= filter.MaxUnitPrice);

               if (filter.MinUnitsInStock.HasValue)
                   query = query.Where(x => x.UnitsInStock >= filter.MinUnitsInStock);

               if (filter.MaxUnitsInStock.HasValue)
                   query = query.Where(x => x.UnitsInStock <= filter.MaxUnitsInStock);

               switch (filter.SortBy)
               {
                   case "id_desc":
                       query = query.OrderByDescending(x => x.ProductID);
                       break;
                   case "name_asc":
                       query = query.OrderBy(x => x.ProductName);
                       break;
                   case "name_desc":
                       query = query.OrderByDescending(x => x.ProductName);
                       break;
                   default:
                       query = query.OrderBy(x => x.ProductID);
                       break;
               }

               var total = await query.CountAsync();
               query = query.Skip((page - 1) * size).Take(size).Include(x => x.Category);

               return await _mapper.ProjectTo<ProductGetAllDTO>(query).ToListAsync();
           });
            return result;
        }




        public async Task<ProductResponseDTO> ProductCreate(ProductCreateDTO obj, CancellationToken ct = default)
        {
            if (obj is null) throw new ArgumentException("Arguments aren't correct");

            var validator = new ProductCreateValidator();
            var res = validator.Validate(obj);
            if (!res.IsValid) throw new ValidationException(res.Errors);

            var entity = _mapper.Map<Product>(obj);

            try
            {
                await _unitOfWork.ProductRepository.AddAsync(entity);
                await _unitOfWork.SaveAsync();

                // best-effort sync
                try
                {
                    await IndexProductAsync(entity, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to index created product {ProductId} in elastic", entity.ProductID);
                }

                CacheKeys.ProductVersion++;

                return new ProductResponseDTO { Message = "Product Created in Service" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during product creation");
                throw new DbUpdateException("Database error", ex);
            }
        }

        public async Task<ProductResponseDTO> ProductUpdateService(int id, ProductUpdateDTO obj, CancellationToken ct = default)
        {
            if (id <= 0) throw new ArgumentException("Invalid ID parameter");
            if (obj is null) throw new ArgumentException("model is not valid");

            var validator = new ProductUpdateValidator();
            var res = validator.Validate(obj);
            if (!res.IsValid) throw new ValidationException(res.Errors);

            var entity = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (entity is null) throw new NotFoundException("Product Not Found");

            _mapper.Map(obj, entity);

            try
            {
                await _unitOfWork.SaveAsync();

                try
                {
                    await IndexProductAsync(entity, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to index updated product {ProductId} in elastic", entity.ProductID);
                }

                CacheKeys.ProductVersion++;

                return new ProductResponseDTO { Message = "Product Updated in Service" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during product update");
                throw new DbUpdateException("Database error during update", ex);
            }
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            if (id <= 0) throw new ValidationException("Id is invalid");

            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product is null) throw new NotFoundException("ID is not correct");

            _unitOfWork.ProductRepository.Remove(product);
            await _unitOfWork.SaveAsync();

            try
            {
                await DeleteProductFromIndexAsync(id, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete in ES failed for product {ProductId}", id);
            }

            CacheKeys.ProductVersion++;
        }

        // ---------------------------
        // ES Index helpers
        // ---------------------------

        private async Task IndexProductAsync(Product entity, CancellationToken ct = default)
        {
            string? categoryName = entity.Category?.CategoryName;

            if (categoryName is null)
            {
                categoryName = await _unitOfWork.CategoryRepository.Query()
                    .AsNoTracking()
                    .Where(c => c.CategoryID == entity.CategoryID)
                    .Select(c => c.CategoryName)
                    .FirstOrDefaultAsync(ct);
            }

            var doc = new ProductElasticDocument
            {
                productID = entity.ProductID,
                productName = entity.ProductName,
                categoryID = entity.CategoryID,
                categoryName = categoryName,
                quantityPerUnit = entity.QuantityPerUnit,
                unitPrice = entity.UnitPrice,
                unitsInStock = entity.UnitsInStock,
                unitsOnOrder = entity.UnitsOnOrder,
                reorderLevel = entity.ReorderLevel,
                discontinued = entity.Discontinued
            };

            var response = await _client.IndexAsync(doc, i => i
                .Index(IndexName)
                .Id(entity.ProductID.ToString()), ct);

            if (!response.IsValidResponse)
                throw new Exception(response.DebugInformation);
        }

        private async Task DeleteProductFromIndexAsync(int id, CancellationToken ct = default)
        {
            var response = await _client.DeleteAsync<ProductElasticDocument>(id.ToString(),
                d => d.Index(IndexName), ct);

            if (!response.IsValidResponse)
                throw new Exception(response.DebugInformation);
        }

        // ---------------------------
        // BULK SYNC (مثل CategoryService)
        // ---------------------------

        public async Task<ProductSyncResult> SyncProductsToElastic(CancellationToken ct = default)
        {
            _logger.LogInformation("Product sync started.");

            var dbCount = await _unitOfWork.ProductRepository.Query()
                .AsNoTracking()
                .CountAsync(ct);

            int page = 0;
            int totalSynced = 0;

            // ensure index exists (اختیاری ولی بهتر)
            var exists = await _client.Indices.ExistsAsync(IndexName, ct);
            if (!exists.IsValidResponse || !exists.Exists)
            {
                var create = await _client.Indices.CreateAsync(IndexName, ct);
                if (!create.IsValidResponse)
                    throw new Exception("Failed to create index: " + create.DebugInformation);
            }

            while (true)
            {
                var chunk = await _unitOfWork.ProductRepository.Query()
                    .AsNoTracking()
                    .Include(p => p.Category)
                    .OrderBy(p => p.ProductID)
                    .Skip(page * BatchSize)
                    .Take(BatchSize)
                    .ToListAsync(ct);

                if (chunk.Count == 0) break;

                // ✅ تبدیل DB Entity -> ProductElasticDocument (نه DTO)
                var docs = chunk.Select(p => new ProductElasticDocument
                {
                    productID = p.ProductID,
                    productName = p.ProductName,
                    categoryID = p.CategoryID,
                    categoryName = p.Category != null ? p.Category.CategoryName : null,
                    quantityPerUnit = p.QuantityPerUnit,
                    unitPrice = p.UnitPrice,
                    unitsInStock = p.UnitsInStock,
                    unitsOnOrder = p.UnitsOnOrder,
                    reorderLevel = p.ReorderLevel,
                    discontinued = p.Discontinued
                }).ToList();

                var bulkRequest = new BulkRequest(IndexName)
                {
                    Operations = new BulkOperationsCollection()
                };

                foreach (var doc in docs)
                {
                    bulkRequest.Operations.Add(new BulkIndexOperation<ProductElasticDocument>(doc)
                    {
                        Id = doc.productID.ToString()
                    });
                }

                var bulkResponse = await _client.BulkAsync(bulkRequest, ct);
                if (!bulkResponse.IsValidResponse)
                    throw new Exception("Bulk failed: " + bulkResponse.DebugInformation);

                if (bulkResponse.Errors == true)
                {
                    var failed = bulkResponse.ItemsWithErrors?.Take(20).ToList();
                    if (failed != null)
                        foreach (var f in failed)
                            _logger.LogError("Bulk item failed: Id={Id} Error={Error}", f.Id, f.Error);

                    throw new Exception("Bulk had item-level errors.");
                }

                totalSynced += docs.Count;
                page++;
            }

            // ✅ خیلی مهم: refresh تا count و search همون لحظه درست شود
            await _client.Indices.RefreshAsync(IndexName, ct);

            var countResponse = await _client.CountAsync<ProductElasticDocument>(c => c.Indices(IndexName), ct);
            if (!countResponse.IsValidResponse)
                throw new Exception("Count failed: " + countResponse.DebugInformation);

            return new ProductSyncResult
            {
                DbCount = dbCount,
                SyncedCount = totalSynced,
                ElasticCount = countResponse.Count
            };
        }



    }
}