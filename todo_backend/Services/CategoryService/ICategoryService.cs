using todo_backend.Dtos.Category;

namespace todo_backend.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto?>> GetCategoriesAsync(int id);
        Task<CategoryDto?> CreateCategoryAsync(ModifyCategoryDto dto, int id);
        Task<CategoryDto?> UpdateCategoryAsync(ModifyCategoryDto dto, int id, int currentUserId);
        Task<bool> DeleteCategoryAsync(int categoryId, int currentUserId);
    }
}
