using todo_backend.Data;
using todo_backend.Dtos.TimelineRecurrenceExceptionDto;
using todo_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace todo_backend.Services.TimelineRecurrenceExceptionService
{
    public class TimelineRecurrenceExceptionService : ITimelineRecurrenceExceptionService
    {
        private readonly AppDbContext _context;

        public TimelineRecurrenceExceptionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TimelineRecurrenceExceptionResponseDto>> GetExceptionsForActivityAsync(int activityId)
        {
            var items = await _context.TimelineRecurrenceExceptions
                .Where(e => e.ActivityId == activityId)
                .OrderBy(e => e.ExceptionDate)
                .ToListAsync();

            return items.Select(e => new TimelineRecurrenceExceptionResponseDto
            {
                ExceptionId = e.ExceptionId,
                ActivityId = e.ActivityId,
                ExceptionDate = e.ExceptionDate,
                NewStartTime = e.NewStartTime,
                NewDurationMinutes = e.NewDurationMinutes,
                Mode = e.Mode
            });
        }

        public async Task<TimelineRecurrenceExceptionResponseDto?> CreateExceptionAsync(TimelineRecurrenceExceptionCreateDto dto)
        {
            var entity = new TimelineRecurrenceException
            {
                ActivityId = dto.ActivityId,
                ExceptionDate = dto.ExceptionDate,
                NewStartTime = dto.NewStartTime,
                NewDurationMinutes = dto.NewDurationMinutes,
                Mode = dto.Mode
            };

            _context.TimelineRecurrenceExceptions.Add(entity);
            await _context.SaveChangesAsync();

            return new TimelineRecurrenceExceptionResponseDto
            {
                ExceptionId = entity.ExceptionId,
                ActivityId = entity.ActivityId,
                ExceptionDate = entity.ExceptionDate,
                NewStartTime = entity.NewStartTime,
                NewDurationMinutes = entity.NewDurationMinutes,
                Mode = entity.Mode
            };
        }

        public async Task<bool> DeleteExceptionAsync(int exceptionId, int userId)
        {
            var entity = await _context.TimelineRecurrenceExceptions
                .Include(e => e.Activity)
                .FirstOrDefaultAsync(e => e.ExceptionId == exceptionId && e.Activity!.OwnerId == userId);

            if (entity == null)
                return false;

            _context.TimelineRecurrenceExceptions.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
