using todo_backend.Dtos.TimelineActivity;

namespace todo_backend.Services.TimelineActivityService
{
    public interface ITimelineActivityService
    {
        Task<IEnumerable<FullTimelineActivityDto?>> GetTimelineActivitiesAsync(int id);
        Task<FullTimelineActivityDto?> GetTimelineActivityByIdAsync(int activityId, int currentUserId);
        Task<FullTimelineActivityDto?> CreateTimelineActivityAsync(CreateTimelineActivityDto dto, int currentUserId);
        Task<FullTimelineActivityDto> UpdateTimelineActivityAsync(int activityId, int currentUserId, UpdateTimelineActivityDto dto);
        Task<bool> DeleteTimelineActivityAsync(int activityId, int currentUserId);
    }
}
