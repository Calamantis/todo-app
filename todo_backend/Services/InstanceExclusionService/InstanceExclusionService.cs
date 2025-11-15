using Microsoft.EntityFrameworkCore;
using todo_backend.Data;
using todo_backend.Dtos.InstanceExclusionDto;
using todo_backend.Models;

namespace todo_backend.Services.InstanceExclusionService
{
    public class InstanceExclusionService : IInstanceExclusionService
    {
        private readonly AppDbContext _context;

        public InstanceExclusionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<InstanceExclusionResponseDto>> GetByUserIdAsync(int userId)
        {
            var exclusions = await _context.InstanceExclusions
                .Where(e => e.UserId == userId)
                .OrderBy(e => e.ExcludedDate)
                .ToListAsync();

            return exclusions.Select(MapToResponse).ToList();
        }

        public async Task<List<InstanceExclusionResponseDto>> GetByActivityAndUserAsync(int userId, int activityId)
        {
            var exclusions = await _context.InstanceExclusions
                .Where(e => e.UserId == userId && e.ActivityId == activityId)
                .OrderBy(e => e.ExcludedDate)
                .ToListAsync();

            return exclusions.Select(MapToResponse).ToList();
        }

        public async Task<InstanceExclusionResponseDto> CreateAsync(int userId, InstanceExclusionCreateDto dto)
        {
            var exclusion = new InstanceExclusion
            {
                UserId = userId,
                ActivityId = dto.ActivityId,
                ExcludedDate = dto.ExcludedDate.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };

            _context.InstanceExclusions.Add(exclusion);
            await _context.SaveChangesAsync();

            return MapToResponse(exclusion);
        }

        public async Task<InstanceExclusionResponseDto> UpdateAsync(int exclusionId, InstanceExclusionUpdateDto dto, int userId)
        {
            var exclusion = await _context.InstanceExclusions
                .FirstOrDefaultAsync(e => e.ExclusionId == exclusionId && e.UserId == userId);

            if (exclusion == null)
                throw new Exception("Exclusion not found");

            exclusion.StartTime = dto.StartTime;
            exclusion.EndTime = dto.EndTime;

            await _context.SaveChangesAsync();

            return MapToResponse(exclusion);
        }

        public async Task<bool> DeleteAsync(int exclusionId, int userId)
        {
            var exclusion = await _context.InstanceExclusions
                .FirstOrDefaultAsync(e => e.ExclusionId == exclusionId && e.UserId == userId);

            if (exclusion == null)
                return false;

            _context.InstanceExclusions.Remove(exclusion);
            await _context.SaveChangesAsync();
            return true;
        }

        private InstanceExclusionResponseDto MapToResponse(InstanceExclusion ex) =>
            new InstanceExclusionResponseDto
            {
                ExclusionId = ex.ExclusionId,
                ActivityId = ex.ActivityId,
                UserId = ex.UserId,
                ExcludedDate = ex.ExcludedDate,
                StartTime = ex.StartTime,
                EndTime = ex.EndTime
            };

    }
}
