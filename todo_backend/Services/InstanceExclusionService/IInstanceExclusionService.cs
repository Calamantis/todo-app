using todo_backend.Dtos.InstanceExclusionDto;

namespace todo_backend.Services.InstanceExclusionService
{
    public interface IInstanceExclusionService
    {
        Task<List<InstanceExclusionResponseDto>> GetByUserIdAsync(int userId);
        Task<List<InstanceExclusionResponseDto>> GetByActivityAndUserAsync(int userId, int activityId);
        Task<InstanceExclusionResponseDto> CreateAsync(int userId, InstanceExclusionCreateDto dto);
        Task<InstanceExclusionResponseDto> UpdateAsync(int exclusionId, InstanceExclusionUpdateDto dto, int userId);
        Task<bool> DeleteAsync(int exclusionId, int userId);
    }
}
