using BulkyWeb.Models.DTO.Category;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Services.Categories
{
    public interface ICategoryService
    {

        Task<List<CategoryGetAllDTO>> GetAllAsync(CategoryFilterDTO filter);
        Task<CategoryResponseDTO> CategoryCreate(CategoryCreateDTO filter);

        Task<CategoryGetByIdDTO> GetByIdAsync(int id);
        Task<CategoryResponseDTO> CategoryUpdateServiece(int id, [FromBody] CategoryUpdateDTO obj);


        Task DeleteAsync(int id, CancellationToken ct = default);



        Task<CategorySyncResult> SyncCategoriesToElastic(CancellationToken cancellationToken = default);
        // Task<CategorySearchResultDTO> ListSearchCategoriesElasticAsync(CategoryFilterDTO filter);
        // Task<CategoryGetByIdDTO?> GetCategoryByIdElasticAsync(int categoryId);

        Task<CategorySearchResultDTO> SearchAsync(CategoryFilterDTO filter, CancellationToken ct = default);
    }
}
