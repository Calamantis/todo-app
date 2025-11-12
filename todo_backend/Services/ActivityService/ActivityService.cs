using Microsoft.EntityFrameworkCore;
using todo_backend.Data;
using todo_backend.Dtos.ActivityDto;
using todo_backend.Models;

namespace todo_backend.Services.ActivityService
{
    public class ActivityService : IActivityService
    {
        private readonly AppDbContext _context;

        public ActivityService(AppDbContext context)
        {
            _context = context;
        }

        // GET: Wszystkie aktywności danego użytkownika
        public async Task<IEnumerable<ActivityResponseDto>> GetActivitiesByUserIdAsync(int currentUserId)
        {
            return await _context.Activities
                .Where(a => a.OwnerId == currentUserId)
                .Select(a => new ActivityResponseDto
                {
                    ActivityId = a.ActivityId,
                    Title = a.Title,
                    Description = a.Description,
                    IsRecurring = a.IsRecurring,
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category != null ? a.Category.Name : null,
                    JoinCode = a.JoinCode
                })
                .ToListAsync();
        }

        // GET: Aktywność po ID
        public async Task<ActivityResponseDto?> GetActivityByIdAsync(int activityId, int currentUserId)
        {
            var activity = await _context.Activities
                .Include(a => a.Category)
                .FirstOrDefaultAsync(a => a.ActivityId == activityId && a.OwnerId == currentUserId);

            if (activity == null) return null;

            return new ActivityResponseDto
            {
                ActivityId = activity.ActivityId,
                Title = activity.Title,
                Description = activity.Description,
                IsRecurring = activity.IsRecurring,
                CategoryId = activity.CategoryId,
                CategoryName = activity.Category?.Name,
                JoinCode = activity.JoinCode
            };
        }

        // POST: Tworzenie nowej aktywności
        public async Task<ActivityResponseDto?> CreateActivityAsync(ActivityCreateDto dto, int currentUserId)
        {
            try
            {
                Category? category = null;
                if (dto.CategoryId.HasValue)
                {
                    category = await _context.Categories.FindAsync(dto.CategoryId.Value);
                    if (category == null) return null; // Jeśli kategoria nie istnieje, zwróć null
                }

                var entity = new Activity
                {
                    OwnerId = currentUserId,
                    Title = dto.Title,
                    Description = dto.Description,
                    IsRecurring = dto.IsRecurring,
                    JoinCode = string.Empty, // Możesz ustawić kod, jeśli masz taką logikę
                    CategoryId = dto.CategoryId
                };

                _context.Activities.Add(entity);
                await _context.SaveChangesAsync();

                return new ActivityResponseDto
                {
                    ActivityId = entity.ActivityId,
                    Title = entity.Title,
                    Description = entity.Description,
                    IsRecurring = entity.IsRecurring,
                    JoinCode = entity.JoinCode,
                    CategoryName = category?.Name
                };
            }
            catch
            {
                return null;
            }
        }

        // PUT: Aktualizacja aktywności
        public async Task<ActivityResponseDto?> UpdateActivityAsync(int activityId, UpdateActivityDto dto, int currentUserId)
        {
            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.ActivityId == activityId && a.OwnerId == currentUserId);

            if (activity == null) return null;

            activity.Title = dto.Title;
            activity.Description = dto.Description;
            activity.IsRecurring = dto.IsRecurring;
            activity.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();

            return new ActivityResponseDto
            {
                ActivityId = activity.ActivityId,
                Title = activity.Title,
                Description = activity.Description,
                IsRecurring = activity.IsRecurring,
                CategoryId = activity.CategoryId,
                CategoryName = activity.Category?.Name,
                JoinCode = activity.JoinCode
            };
        }

        // DELETE: Usuwanie aktywności
        public async Task<bool> DeleteActivityAsync(int activityId, int currentUserId)
        {
            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.ActivityId == activityId && a.OwnerId == currentUserId);

            if (activity == null) return false;

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
