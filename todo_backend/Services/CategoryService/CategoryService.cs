using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using todo_backend.Data;
using todo_backend.Dtos.Category;
using todo_backend.Dtos.Friendship;
using todo_backend.Models;

namespace todo_backend.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        //GET przeglądanie (swoich) kategorii
        public async Task<IEnumerable<CategoryDto?>> GetCategoriesAsync(int id)
        {
            return await _context.Categories
                .Where(c => c.UserId == id)
                .Select(c => new CategoryDto
                {
                    Name = c.Name,
                    ColorHex = c.ColorHex
                })
                .ToListAsync();
        }

        //POST dodawanie kategorii
        public async Task<CategoryDto?> CreateCategoryAsync(CategoryDto dto, int id)
        {
            var exists = await _context.Categories
                .AnyAsync(c => c.UserId == id && c.Name == dto.Name);

            if (exists) return null;

            var category = new Category
            {
                UserId = id,
                Name = dto.Name,
                ColorHex = dto.ColorHex,
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryDto
            {
                Name = dto.Name,
                ColorHex = dto.ColorHex
            };
        }

        //PUT modyfikowanie kategorii
        public async Task<CategoryDto> UpdateCategoryAsync(CategoryDto dto, int id, int currentUserId)
        {
            var category = await _context.Categories.FindAsync(id); //id kategorii a nie uzytkownika
            if (category == null) return null!;

            if (category.UserId != currentUserId)
                throw new UnauthorizedAccessException("You cannot edit someone else's category.");

            var nameExists = await _context.Categories
                .AnyAsync(c => c.UserId == currentUserId && c.Name == dto.Name && c.CategoryId != id);
            if (nameExists)
                throw new InvalidOperationException("Category name already exists.");

            category.Name = dto.Name;
            category.ColorHex = dto.ColorHex;

            await _context.SaveChangesAsync();

            return new CategoryDto
            {
                Name = dto.Name,
                ColorHex = dto.ColorHex
            };
        }

        //DELETE usuwanie kategorii
        public async Task<bool> DeleteCategoryAsync(int categoryId, bool deleteActivities, int currentUserId)
        {
            var category = await _context.Categories
                .Include(c => c.TimelineActivities)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

            if (category == null) return false;

            if (category.UserId != currentUserId)
                return false; // nie jesteś właścicielem -> odmowa

            if (deleteActivities)
            {
                // tryb 1: usuń wszystkie aktywności
                _context.TimelineActivities.RemoveRange(category.TimelineActivities);
            }

            // tryb 2: samo usunięcie kategorii → SetNull zadba o FK
            _context.Categories.Remove(category);

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
