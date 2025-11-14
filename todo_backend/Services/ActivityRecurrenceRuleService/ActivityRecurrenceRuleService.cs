using todo_backend.Data;
using todo_backend.Dtos.ActivityRecurrenceRuleDto;
using todo_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace todo_backend.Services.ActivityRecurrenceRuleService
{
    public class ActivityRecurrenceRuleService : IActivityRecurrenceRuleService
    {

        private readonly AppDbContext _context;

        public ActivityRecurrenceRuleService(AppDbContext context)
        {
            _context = context;
        }

        //// GET: Wszystkie reguły rekurencji
        //public async Task<IEnumerable<ActivityRecurrenceRuleDto>> GetAllRecurrenceRulesAsync()
        //{
        //    return await _context.ActivityRecurrenceRules
        //        .Select(r => new ActivityRecurrenceRuleDto
        //        {
        //            RecurrenceRuleId = r.RecurrenceRuleId,
        //            ActivityId = r.ActivityId,
        //            Type = r.Type,
        //            DaysOfWeek = r.DaysOfWeek,
        //            DaysOfMonth = r.DaysOfMonth,
        //            StartTime = r.StartTime,
        //            EndTime = r.EndTime,
        //            DateRangeStart = r.DateRangeStart,
        //            DateRangeEnd = r.DateRangeEnd,
        //            DurationMinutes = r.DurationMinutes,
        //            IsActive = r.IsActive
        //        })
        //        .ToListAsync();
        //}

        // GET: Reguły rekurencji po ID użytkownika
        public async Task<IEnumerable<ActivityRecurrenceRuleDto>> GetRecurrenceRulesByUserIdAsync(int userId)
        {
            return await _context.ActivityRecurrenceRules
                .Where(r => r.Activity.OwnerId == userId)
                .Select(r => new ActivityRecurrenceRuleDto
                {
                    RecurrenceRuleId = r.RecurrenceRuleId,
                    ActivityId = r.ActivityId,
                    Type = r.Type,
                    DaysOfWeek = r.DaysOfWeek,
                    DaysOfMonth = r.DaysOfMonth,
                    DayOfYear = r.DayOfYear,
                    Interval = r.Interval,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    DateRangeStart = r.DateRangeStart,
                    DateRangeEnd = r.DateRangeEnd,
                    DurationMinutes = r.DurationMinutes,
                    IsActive = r.IsActive
                })
                .ToListAsync();
        }

        // GET: Reguły rekurencji po ID aktywności
        public async Task<IEnumerable<ActivityRecurrenceRuleDto>> GetRecurrenceRulesByActivityIdAsync(int activityId, int userId)
        {
            var activity = await _context.Activities
            .FirstOrDefaultAsync(a => a.ActivityId == activityId && a.OwnerId == userId);

            if (activity == null)
            {
                return null; // Jeśli aktywność nie należy do użytkownika, zwróć null
            }

            return await _context.ActivityRecurrenceRules
                .Where(r => r.ActivityId == activityId)
                .Select(r => new ActivityRecurrenceRuleDto
                {
                    RecurrenceRuleId = r.RecurrenceRuleId,
                    ActivityId = r.ActivityId,
                    Type = r.Type,
                    DaysOfWeek = r.DaysOfWeek,
                    DaysOfMonth = r.DaysOfMonth,
                    DayOfYear = r.DayOfYear,
                    Interval = r.Interval,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    DateRangeStart = r.DateRangeStart,
                    DateRangeEnd = r.DateRangeEnd,
                    DurationMinutes = r.DurationMinutes,
                    IsActive = r.IsActive
                })
                .ToListAsync();
        }

        // POST: Tworzenie nowej reguły rekurencyjnej
        public async Task<ActivityRecurrenceRuleDto?> CreateRecurrenceRuleAsync(ActivityRecurrenceRuleDto dto, int userId)
        {
            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.ActivityId == dto.ActivityId && a.OwnerId == userId);

            if (activity == null)
            {
                return null; // Jeśli aktywność nie należy do użytkownika, zwróć null
            }

            var entity = new ActivityRecurrenceRule
            {
                ActivityId = dto.ActivityId,
                Type = dto.Type,
                DaysOfWeek = dto.DaysOfWeek,
                DaysOfMonth = dto.DaysOfMonth,
                DayOfYear = dto.DayOfYear,
                Interval = dto.Interval,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                DateRangeStart = dto.DateRangeStart,
                DateRangeEnd = dto.DateRangeEnd,
                DurationMinutes = dto.DurationMinutes,
                IsActive = dto.IsActive
            };

            _context.ActivityRecurrenceRules.Add(entity);
            await _context.SaveChangesAsync();

            return new ActivityRecurrenceRuleDto
            {
                RecurrenceRuleId = entity.RecurrenceRuleId,
                ActivityId = entity.ActivityId,
                Type = entity.Type,
                DaysOfWeek = entity.DaysOfWeek,
                DaysOfMonth = entity.DaysOfMonth,
                DayOfYear = entity.DayOfYear,
                Interval = entity.Interval,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                DateRangeStart = entity.DateRangeStart,
                DateRangeEnd = entity.DateRangeEnd,
                DurationMinutes = entity.DurationMinutes,
                IsActive = entity.IsActive
            };
        }

        // PUT: Aktualizacja reguły rekurencyjnej
        public async Task<ActivityRecurrenceRuleDto?> UpdateRecurrenceRuleAsync(int userId, int recurrenceRuleId, ActivityRecurrenceRuleDto dto)
        {
            var rule = await _context.ActivityRecurrenceRules
                .Include(r => r.Activity)
                .FirstOrDefaultAsync(r => r.RecurrenceRuleId == recurrenceRuleId);

            if (rule == null || rule.Activity.OwnerId != userId)
            {
                return null; // Jeśli reguła nie należy do użytkownika, zwróć null
            }

            rule.Type = dto.Type;
            rule.DaysOfWeek = dto.DaysOfWeek;
            rule.DaysOfMonth = dto.DaysOfMonth;
            rule.DayOfYear = dto.DayOfYear;
            rule.Interval = dto.Interval;
            rule.StartTime = dto.StartTime;
            rule.EndTime = dto.EndTime;
            rule.DateRangeStart = dto.DateRangeStart;
            rule.DateRangeEnd = dto.DateRangeEnd;
            rule.DurationMinutes = dto.DurationMinutes;
            rule.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return new ActivityRecurrenceRuleDto
            {
                RecurrenceRuleId = rule.RecurrenceRuleId,
                ActivityId = rule.ActivityId,
                Type = rule.Type,
                DaysOfWeek = rule.DaysOfWeek,
                DaysOfMonth = rule.DaysOfMonth,
                DayOfYear = rule.DayOfYear,
                Interval = rule.Interval,
                StartTime = rule.StartTime,
                EndTime = rule.EndTime,
                DateRangeStart = rule.DateRangeStart,
                DateRangeEnd = rule.DateRangeEnd,
                DurationMinutes = rule.DurationMinutes,
                IsActive = rule.IsActive
            };
        }

        // DELETE: Usuwanie reguły rekurencyjnej
        public async Task<bool> DeleteRecurrenceRuleAsync(int recurrenceRuleId, int userId)
        {
            var rule = await _context.ActivityRecurrenceRules
                    .Include(r => r.Activity)
                    .FirstOrDefaultAsync(r => r.RecurrenceRuleId == recurrenceRuleId);

            if (rule == null) return false;

            _context.ActivityRecurrenceRules.Remove(rule);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
