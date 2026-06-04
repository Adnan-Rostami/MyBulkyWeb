using BulkyWeb.Models.DTO.Product;

namespace BulkyWeb.Services.Products
{
    public interface IProductService
    {
        Task<List<ProductGetAllDTO>> GetAllAsync(ProductFilterDTO filter);
        Task<ProductResponseDTO> ProductCreate(ProductCreateDTO obj);

        Task<ProductGetByIdDTO> GetByIdAsync(int id);
        Task<ProductResponseDTO> ProductUpdateService(int id, ProductUpdateDTO obj);


        Task DeleteAsync(int id);
        // Task<object> SyncProductsToElastic();

        Task<ProductSearchResultDTO> SearchProductsElasticAsync(ProductFilterDTO filter);

        Task<object> GetProductsElasticStatusAsync();
        // Task<ProductGetByIdDTO?> GetProductByIdElasticAsync(int productId, CancellationToken ct = default);



        //Task IndexProductAsync(ProductGetAllDTO product);
        // Task DeleteProductAsync(int productId);
        // Task<ProductSearchResultDTO?> SearchAsync(ProductFilterDTO filter);








        Task<ProductSearchResultDTO> SearchAsync(ProductFilterDTO filter, CancellationToken ct = default);

        Task<ProductGetByIdDTO> GetByIdAsync(int id, CancellationToken ct = default);

        Task<ProductResponseDTO> ProductCreate(ProductCreateDTO obj, CancellationToken ct = default);
        Task<ProductResponseDTO> ProductUpdateService(int id, ProductUpdateDTO obj, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);

        Task<ProductSyncResult> SyncProductsToElastic(CancellationToken ct = default);
    }
}
