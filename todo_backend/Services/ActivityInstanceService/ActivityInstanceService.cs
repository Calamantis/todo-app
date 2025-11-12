using Microsoft.EntityFrameworkCore;
using todo_backend.Data;
using todo_backend.Dtos.ActivityInstance;
using todo_backend.Models;

namespace todo_backend.Services.ActivityInstanceService
{
    public class ActivityInstanceService : IActivityInstanceService
    {
        private readonly AppDbContext _context;

        public ActivityInstanceService(AppDbContext context)
        {
            _context = context;
        }

        // GET: Wszystkie instancje użytkownika
        public async Task<IEnumerable<ActivityInstanceDto>> GetAllInstancesByUserIdAsync(int userId)
        {
            return await _context.ActivityInstances
                .Where(ai => ai.Activity.OwnerId == userId) // Sprawdzenie, czy instancje należą do użytkownika
                .Select(ai => new ActivityInstanceDto
                {
                    InstanceId = ai.InstanceId,
                    ActivityId = ai.ActivityId,
                    RecurrenceRuleId = ai.RecurrenceRuleId,
                    OccurrenceDate = ai.OccurrenceDate,
                    StartTime = ai.StartTime,
                    EndTime = ai.EndTime,
                    DurationMinutes = ai.DurationMinutes,
                    IsActive = ai.IsActive,
                    DidOccur = ai.DidOccur,
                    IsException = ai.IsException
                })
                .ToListAsync();
        }

        // GET: Instancje po ID aktywności
        public async Task<IEnumerable<ActivityInstanceDto>> GetInstancesByActivityIdAsync(int activityId, int userId)
        {
            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.ActivityId == activityId && a.OwnerId == userId);

            if (activity == null)
            {
                return null; // Jeśli aktywność nie należy do użytkownika, zwróć null
            }

            return await _context.ActivityInstances
                .Where(ai => ai.ActivityId == activityId)
                .Select(ai => new ActivityInstanceDto
                {
                    InstanceId = ai.InstanceId,
                    ActivityId = ai.ActivityId,
                    RecurrenceRuleId = ai.RecurrenceRuleId,
                    OccurrenceDate = ai.OccurrenceDate,
                    StartTime = ai.StartTime,
                    EndTime = ai.EndTime,
                    DurationMinutes = ai.DurationMinutes,
                    IsActive = ai.IsActive,
                    DidOccur = ai.DidOccur,
                    IsException = ai.IsException
                })
                .ToListAsync();
        }

        //// GET: Instancje w określonym przedziale dat
        //public async Task<IEnumerable<ActivityInstanceDto>> GetInstancesByDateRangeAsync(DateTime startDate, DateTime endDate, int userId)
        //{
        //    return await _context.ActivityInstances
        //        .Where(ai => ai.Activity.OwnerId == userId && ai.OccurrenceDate >= startDate && ai.OccurrenceDate <= endDate)
        //        .Select(ai => new ActivityInstanceDto
        //        {
        //            InstanceId = ai.InstanceId,
        //            ActivityId = ai.ActivityId,
        //            RecurrenceRuleId = ai.RecurrenceRuleId,
        //            OccurrenceDate = ai.OccurrenceDate,
        //            StartTime = ai.StartTime,
        //            EndTime = ai.EndTime,
        //            DurationMinutes = ai.DurationMinutes,
        //            IsActive = ai.IsActive,
        //            DidOccur = ai.DidOccur,
        //            IsException = ai.IsException
        //        })
        //        .ToListAsync();
        //}

        // POST: Tworzenie nowej instancji
        public async Task<ActivityInstanceDto?> CreateInstanceAsync(ActivityInstanceDto dto, int userId)
        {
            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.ActivityId == dto.ActivityId && a.OwnerId == userId);

            if (activity == null)
            {
                return null; // Jeśli aktywność nie należy do użytkownika, zwróć null
            }

            var entity = new ActivityInstance
            {
                ActivityId = dto.ActivityId,
                RecurrenceRuleId = dto.RecurrenceRuleId,
                OccurrenceDate = dto.OccurrenceDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                DurationMinutes = dto.DurationMinutes,
                IsActive = dto.IsActive,
                DidOccur = dto.DidOccur,
                IsException = dto.IsException
            };

            _context.ActivityInstances.Add(entity);
            await _context.SaveChangesAsync();

            return new ActivityInstanceDto
            {
                InstanceId = entity.InstanceId,
                ActivityId = entity.ActivityId,
                RecurrenceRuleId = entity.RecurrenceRuleId,
                OccurrenceDate = entity.OccurrenceDate,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                DurationMinutes = entity.DurationMinutes,
                IsActive = entity.IsActive,
                DidOccur = entity.DidOccur,
                IsException = entity.IsException
            };
        }

        // PUT: Aktualizacja instancji
        public async Task<ActivityInstanceDto?> UpdateInstanceAsync(int instanceId, ActivityInstanceDto dto, int userId)
        {
            var instance = await _context.ActivityInstances
                .Include(ai => ai.Activity)
                .FirstOrDefaultAsync(ai => ai.InstanceId == instanceId);

            if (instance == null || instance.Activity.OwnerId != userId)
            {
                return null; // Jeśli instancja nie należy do użytkownika, zwróć null
            }

            instance.OccurrenceDate = dto.OccurrenceDate;
            instance.StartTime = dto.StartTime;
            instance.EndTime = dto.EndTime;
            instance.DurationMinutes = dto.DurationMinutes;
            instance.IsActive = dto.IsActive;
            instance.DidOccur = dto.DidOccur;
            instance.IsException = dto.IsException;

            await _context.SaveChangesAsync();

            return new ActivityInstanceDto
            {
                InstanceId = instance.InstanceId,
                ActivityId = instance.ActivityId,
                RecurrenceRuleId = instance.RecurrenceRuleId,
                OccurrenceDate = instance.OccurrenceDate,
                StartTime = instance.StartTime,
                EndTime = instance.EndTime,
                DurationMinutes = instance.DurationMinutes,
                IsActive = instance.IsActive,
                DidOccur = instance.DidOccur,
                IsException = instance.IsException
            };
        }

        // DELETE: Usuwanie instancji
        public async Task<bool> DeleteInstanceAsync(int instanceId, int userId)
        {
            var instance = await _context.ActivityInstances
                .Include(ai => ai.Activity)
                .FirstOrDefaultAsync(ai => ai.InstanceId == instanceId);

            if (instance == null || instance.Activity.OwnerId != userId)
            {
                return false; // Jeśli instancja nie należy do użytkownika, zwróć false
            }

            _context.ActivityInstances.Remove(instance);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
