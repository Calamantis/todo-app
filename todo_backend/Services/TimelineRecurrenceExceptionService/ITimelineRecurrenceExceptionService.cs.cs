using todo_backend.Dtos.TimelineRecurrenceExceptionDto;
namespace todo_backend.Services.TimelineRecurrenceExceptionService
{
    public interface ITimelineRecurrenceExceptionService
    {
        Task<IEnumerable<TimelineRecurrenceExceptionResponseDto>> GetAllAsync(int activityId);
        Task<TimelineRecurrenceExceptionResponseDto?> GetByIdAsync(int id);
        Task<TimelineRecurrenceExceptionResponseDto> CreateAsync(TimelineRecurrenceExceptionCreateDto dto);
        Task<TimelineRecurrenceExceptionResponseDto?> UpdateAsync(int id, TimelineRecurrenceExceptionUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
