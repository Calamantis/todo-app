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

        public async Task<IEnumerable<TimelineRecurrenceExceptionResponseDto>> GetAllAsync(int activityId)
        {
            return await _context.TimelineRecurrenceExceptions
                .Where(e => e.ActivityId == activityId)
                .OrderBy(e => e.ExceptionDate)
                .Select(e => new TimelineRecurrenceExceptionResponseDto
                {
                    ExceptionId = e.ExceptionId,
                    ActivityId = e.ActivityId,
                    ExceptionDate = e.ExceptionDate,
                    NewStartTime = e.NewStartTime,
                    NewDurationMinutes = e.NewDurationMinutes,
                    IsSkipped = e.IsSkipped
                }).ToListAsync();
        }

        public async Task<TimelineRecurrenceExceptionResponseDto?> GetByIdAsync(int id)
        {
            var e = await _context.TimelineRecurrenceExceptions.FindAsync(id);
            if (e == null) return null;

            return new TimelineRecurrenceExceptionResponseDto
            {
                ExceptionId = e.ExceptionId,
                ActivityId = e.ActivityId,
                ExceptionDate = e.ExceptionDate,
                NewStartTime = e.NewStartTime,
                NewDurationMinutes = e.NewDurationMinutes,
                IsSkipped = e.IsSkipped
            };
        }

        public async Task<TimelineRecurrenceExceptionResponseDto> CreateAsync(TimelineRecurrenceExceptionCreateDto dto)
        {
            var entity = new TimelineRecurrenceException
            {
                ActivityId = dto.ActivityId,
                ExceptionDate = dto.ExceptionDate,
                NewStartTime = dto.NewStartTime,
                NewDurationMinutes = dto.NewDurationMinutes,
                IsSkipped = dto.IsSkipped
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
                IsSkipped = entity.IsSkipped
            };
        }

        public async Task<TimelineRecurrenceExceptionResponseDto?> UpdateAsync(int id, TimelineRecurrenceExceptionUpdateDto dto)
        {
            var entity = await _context.TimelineRecurrenceExceptions.FindAsync(id);
            if (entity == null) return null;

            entity.NewStartTime = dto.NewStartTime;
            entity.NewDurationMinutes = dto.NewDurationMinutes;
            entity.IsSkipped = dto.IsSkipped;

            await _context.SaveChangesAsync();

            return new TimelineRecurrenceExceptionResponseDto
            {
                ExceptionId = entity.ExceptionId,
                ActivityId = entity.ActivityId,
                ExceptionDate = entity.ExceptionDate,
                NewStartTime = entity.NewStartTime,
                NewDurationMinutes = entity.NewDurationMinutes,
                IsSkipped = entity.IsSkipped
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.TimelineRecurrenceExceptions.FindAsync(id);
            if (entity == null) return false;

            _context.TimelineRecurrenceExceptions.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
