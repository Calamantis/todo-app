using todo_backend.Dtos.Category;

namespace todo_backend.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto?>> GetCategoriesAsync(int id);
        Task<CategoryDto?> CreateCategoryAsync(CategoryDto dto, int id);
        Task<CategoryDto?> UpdateCategoryAsync(CategoryDto dto, int id, int currentUserId);
        Task<bool> DeleteCategoryAsync(int categoryId, bool deleteActivities, int currentUserId);
    }
}
