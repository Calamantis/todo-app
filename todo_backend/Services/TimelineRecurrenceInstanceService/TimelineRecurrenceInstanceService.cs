using Microsoft.EntityFrameworkCore;
using todo_backend.Data;
using todo_backend.Dtos.TimelineRecurrenceInstanceDto;
using todo_backend.Models;

namespace todo_backend.Services.TimelineRecurrenceInstanceService
{
    public class TimelineRecurrenceInstanceService : ITimelineRecurrenceInstanceService
    {
        private readonly AppDbContext _context;

        public TimelineRecurrenceInstanceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TimelineRecurrenceInstanceResponseDto>> GetAllAsync(int activityId)
        {
            return await _context.TimelineRecurrenceInstances
                .Where(i => i.ActivityId == activityId)
                .OrderBy(i => i.OccurrenceDate)
                .Select(i => new TimelineRecurrenceInstanceResponseDto
                {
                    InstanceId = i.InstanceId,
                    ActivityId = i.ActivityId,
                    OccurrenceDate = i.OccurrenceDate,
                    StartTime = i.StartTime,
                    EndTime = i.EndTime,
                    DurationMinutes = i.DurationMinutes,
                    IsCompleted = i.IsCompleted
                })
                .ToListAsync();
        }

        public async Task<TimelineRecurrenceInstanceResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _context.TimelineRecurrenceInstances.FindAsync(id);
            if (entity == null) return null;

            return new TimelineRecurrenceInstanceResponseDto
            {
                InstanceId = entity.InstanceId,
                ActivityId = entity.ActivityId,
                OccurrenceDate = entity.OccurrenceDate,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                DurationMinutes = entity.DurationMinutes,
                IsCompleted = entity.IsCompleted
            };
        }

        public async Task<TimelineRecurrenceInstanceResponseDto> CreateAsync(TimelineRecurrenceInstanceCreateDto dto)
        {
            var entity = new TimelineRecurrenceInstance
            {
                ActivityId = dto.ActivityId,
                OccurrenceDate = dto.OccurrenceDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                DurationMinutes = dto.DurationMinutes,
                IsCompleted = dto.IsCompleted
            };

            _context.TimelineRecurrenceInstances.Add(entity);
            await _context.SaveChangesAsync();

            return new TimelineRecurrenceInstanceResponseDto
            {
                InstanceId = entity.InstanceId,
                ActivityId = entity.ActivityId,
                OccurrenceDate = entity.OccurrenceDate,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                DurationMinutes = entity.DurationMinutes,
                IsCompleted = entity.IsCompleted
            };
        }

        public async Task<TimelineRecurrenceInstanceResponseDto?> UpdateAsync(int id, TimelineRecurrenceInstanceUpdateDto dto)
        {   
            var entity = await _context.TimelineRecurrenceInstances.FindAsync(id);
            if (entity == null) return null;

            if (dto.StartTime.HasValue) entity.StartTime = dto.StartTime.Value;
            if (dto.EndTime.HasValue) entity.EndTime = dto.EndTime.Value;
            if (dto.DurationMinutes.HasValue) entity.DurationMinutes = dto.DurationMinutes.Value;
            if (dto.IsCompleted.HasValue) entity.IsCompleted = dto.IsCompleted.Value;

            await _context.SaveChangesAsync();

            return new TimelineRecurrenceInstanceResponseDto
            {
                InstanceId = entity.InstanceId,
                ActivityId = entity.ActivityId,
                OccurrenceDate = entity.OccurrenceDate,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                DurationMinutes = entity.DurationMinutes,
                IsCompleted = entity.IsCompleted
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.TimelineRecurrenceInstances.FindAsync(id);
            if (entity == null) return false;

            _context.TimelineRecurrenceInstances.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
