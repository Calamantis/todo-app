using todo_backend.Dtos.TimelineRecurrenceInstanceDto;

namespace todo_backend.Services.TimelineRecurrenceInstanceService
{
    public interface ITimelineRecurrenceInstanceService
    {
        Task<IEnumerable<TimelineRecurrenceInstanceResponseDto>> GetAllAsync(int activityId);
        Task<TimelineRecurrenceInstanceResponseDto?> GetByIdAsync(int id);
        Task<TimelineRecurrenceInstanceResponseDto> CreateAsync(TimelineRecurrenceInstanceCreateDto dto);
        Task<TimelineRecurrenceInstanceResponseDto?> UpdateAsync(int id, TimelineRecurrenceInstanceUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
