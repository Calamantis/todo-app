using todo_backend.Data;
using todo_backend.Dtos.ActivityStorage;
using todo_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace todo_backend.Services.ActivityStorage
{
    public class ActivityStorageService : IActivityStorageService
    {
        private readonly AppDbContext _context;

        public ActivityStorageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActivityStorageDto>> GetUserTemplatesAsync(int userId)
        {
            return await _context.ActivityStorage
                .Where(t => t.UserId == userId)
                .Include(t => t.Category)
                .Select(t => new ActivityStorageDto
                {
                    TemplateId = t.TemplateId,
                    Title = t.Title,
                    Description = t.Description,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category != null ? t.Category.Name : null,
                    IsRecurring = t.IsRecurring,
                    RecurrenceRule = t.RecurrenceRule
                })
                .ToListAsync();
        }

        public async Task<ActivityStorageDto?> GetTemplateByIdAsync(int templateId, int userId)
        {
            var t = await _context.ActivityStorage
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.TemplateId == templateId && t.UserId == userId);

            if (t == null) return null;

            return new ActivityStorageDto
            {
                TemplateId = t.TemplateId,
                Title = t.Title,
                Description = t.Description,
                CategoryId = t.CategoryId,
                CategoryName = t.Category?.Name,
                IsRecurring = t.IsRecurring,
                RecurrenceRule = t.RecurrenceRule
            };
        }

        public async Task<ActivityStorageDto?> CreateTemplateAsync(ActivityStorageDto dto, int userId)
        {
            var entity = new Models.ActivityStorage
            {
                UserId = userId,
                Title = dto.Title,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                IsRecurring = dto.IsRecurring,
                RecurrenceRule = dto.RecurrenceRule
            };

            _context.ActivityStorage.Add(entity);
            await _context.SaveChangesAsync();

            dto.TemplateId = entity.TemplateId;
            return dto;
        }

        public async Task<ActivityStorageDto?> UpdateTemplateAsync(int templateId, ActivityStorageDto dto, int userId)
        {
            var entity = await _context.ActivityStorage
                .FirstOrDefaultAsync(t => t.TemplateId == templateId && t.UserId == userId);

            if (entity == null) return null;

            entity.Title = dto.Title;
            entity.Description = dto.Description;
            entity.CategoryId = dto.CategoryId;
            entity.IsRecurring = dto.IsRecurring;
            entity.RecurrenceRule = dto.RecurrenceRule;

            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<bool> DeleteTemplateAsync(int templateId, int userId)
        {
            var entity = await _context.ActivityStorage
                .FirstOrDefaultAsync(t => t.TemplateId == templateId && t.UserId == userId);

            if (entity == null) return false;

            _context.ActivityStorage.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
