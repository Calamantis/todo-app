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
                .Where(c => c.UserId == id || c.IsSystem == true)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    ColorHex = c.ColorHex
                })
                .ToListAsync();
        }

        //POST dodawanie kategorii
        public async Task<CategoryDto?> CreateCategoryAsync(ModifyCategoryDto dto, int id)
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
        public async Task<CategoryDto?> UpdateCategoryAsync(ModifyCategoryDto dto, int id, int currentUserId)
        {
            var category = await _context.Categories.FindAsync(id); //id kategorii a nie uzytkownika
            if (category == null) return null;

            if (category.UserId != currentUserId)
                return null;

            var nameExists = await _context.Categories
                .AnyAsync(c => c.UserId == currentUserId && c.Name == dto.Name && c.CategoryId != id);
            if (nameExists)
                return null;

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
        public async Task<bool> DeleteCategoryAsync(int categoryId, int currentUserId)
        {
            var category = await _context.Categories
                .Include(c => c.TimelineActivities)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

            if (category == null) return false;

            if (category.UserId != currentUserId)
                return false; // nie jesteś właścicielem -> odmowa

            _context.Categories.Remove(category);

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
