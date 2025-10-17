using Microsoft.EntityFrameworkCore;

using todo_backend.Data;
using todo_backend.Dtos.Friendship;
using todo_backend.Dtos.TimelineActivity;
using todo_backend.Models;

namespace todo_backend.Services.TimelineActivityService
{
    public class TimelineActivityService : ITimelineActivityService
    {
        private readonly AppDbContext _context;

        public TimelineActivityService(AppDbContext context) 
        { 
            _context = context;
        }

        //GET przeglądanie (swoich) aktywnosci
        public async Task<IEnumerable<FullTimelineActivityDto?>> GetTimelineActivitiesAsync(int id)
        {
            return await _context.TimelineActivities
                        .Where(t => t.OwnerId == id)
                        .Select(t => new FullTimelineActivityDto
                        {
                            ActivityId = t.ActivityId,
                            Title = t.Title,
                            Description = t.Description,
                            StartTime = t.Start_time,
                            EndTime = t.End_time,
                            IsRecurring = t.Is_recurring,
                            RecurrenceRule = t.Recurrence_rule,
                            CategoryName = t.Category != null ? t.Category.Name : null,
                            JoinCode = t.JoinCode
                        })
                        .ToListAsync();
        }

        //GET po id
        public async Task<FullTimelineActivityDto?> GetTimelineActivityByIdAsync(int activityId, int currentUserId)
        {
            var activity = await _context.TimelineActivities
                            .Include(t => t.Category)
                            .FirstOrDefaultAsync(t => t.ActivityId == activityId && t.OwnerId == currentUserId);

            if (activity == null) return null;

            return new FullTimelineActivityDto
            {
                ActivityId = activity.ActivityId,
                Title = activity.Title,
                Description = activity.Description,
                StartTime = activity.Start_time,
                EndTime = activity.End_time,
                IsRecurring = activity.Is_recurring,
                RecurrenceRule = activity.Recurrence_rule,
                CategoryName = activity.Category != null ? activity.Category.Name : null,
                JoinCode = activity.JoinCode
            };

        }

        //POST stworzenie aktywnosci
        public async Task<FullTimelineActivityDto?> CreateTimelineActivityAsync(CreateTimelineActivityDto dto, int currentUserId)
        {
            try
            {
                // Sprawdź czy kategoria istnieje (jeżeli została podana)
                Category? category = null;
                if (dto.CategoryId.HasValue)
                {
                    category = await _context.Categories.FindAsync(dto.CategoryId.Value);
                    if (category == null)
                        return null; // zwracamy null zamiast rzucać wyjątkiem
                }

                var entity = new TimelineActivity
                {
                    OwnerId = currentUserId,
                    Title = dto.Title,
                    Description = dto.Description,
                    CategoryId = dto.CategoryId,
                    Start_time = dto.StartTime,
                    End_time = dto.EndTime,
                    Is_recurring = dto.IsRecurring,
                    Recurrence_rule = dto.RecurrenceRule,
                    JoinCode = GenerateJoinCode()
                };

                _context.TimelineActivities.Add(entity);
                await _context.SaveChangesAsync();

                // 🔹 Dodaj ownera do ActivityMembers
                var ownerMember = new ActivityMembers
                {
                    ActivityId = entity.ActivityId,
                    UserId = currentUserId,
                    Role = "owner",
                    Status = "accepted" // automatycznie zaakceptowany
                };
                _context.ActivityMembers.Add(ownerMember);
                await _context.SaveChangesAsync();

                return new FullTimelineActivityDto
                {
                    ActivityId = entity.ActivityId,
                    Title = entity.Title,
                    Description = entity.Description,
                    StartTime = entity.Start_time,
                    EndTime = entity.End_time,
                    IsRecurring = entity.Is_recurring,
                    RecurrenceRule = entity.Recurrence_rule,
                    CategoryName = category?.Name
                };
            }
            catch
            {
                // Złap każdy błąd runtime i zwróć null
                return null;
            }
        }

        //PUT modyfikacja aktywności
        public async Task<FullTimelineActivityDto> UpdateTimelineActivityAsync (int activityId, int currentUserId, UpdateTimelineActivityDto dto)
        {
            var entity = await _context.TimelineActivities
                .FirstOrDefaultAsync(t => t.ActivityId == activityId && t.OwnerId == currentUserId);

            if (entity == null) return null;

            entity.Title = dto.Title;
            entity.Description = dto.Description;
            entity.CategoryId = dto.CategoryId;
            entity.Start_time = dto.StartTime;
            entity.End_time = dto.EndTime;
            entity.Is_recurring = dto.IsRecurring;
            entity.Recurrence_rule = dto.RecurrenceRule;

            await _context.SaveChangesAsync();

            var category = await _context.Categories.FindAsync(dto.CategoryId);

            return new FullTimelineActivityDto
            {
                ActivityId = entity.ActivityId,
                Title = entity.Title,
                Description = entity.Description,
                StartTime = entity.Start_time,
                EndTime = entity.End_time,
                IsRecurring = entity.Is_recurring,
                RecurrenceRule = entity.Recurrence_rule,
                CategoryName = category?.Name,
                JoinCode = entity.JoinCode
            };

        }

        //DELETE usunięcie aktywnosci
        public async Task<bool> DeleteTimelineActivityAsync(int activityId, int currentUserId)
        {
            var entity = await _context.TimelineActivities
                .FirstOrDefaultAsync(t => t.ActivityId == activityId && t.OwnerId == currentUserId);

            if (entity == null) return false;


            // Usuń zaproszenia związane z tą aktywnością
            var activityMembers = _context.ActivityMembers
                .Where(am => am.ActivityId == activityId);
            _context.ActivityMembers.RemoveRange(activityMembers);

            _context.TimelineActivities.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        private static string GenerateJoinCode(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
        }

    }
}
