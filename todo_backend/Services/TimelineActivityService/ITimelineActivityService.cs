using todo_backend.Dtos.ActivitySuggestionDto;
using todo_backend.Dtos.TimelineActivity;

namespace todo_backend.Services.TimelineActivityService
{
    public interface ITimelineActivityService
    {
        Task<IEnumerable<FullTimelineActivityDto?>> GetTimelineActivitiesAsync(int id);
        Task<FullTimelineActivityDto?> GetTimelineActivityByIdAsync(int activityId, int currentUserId);
        Task<FullTimelineActivityDto?> CreateTimelineActivityAsync(CreateTimelineActivityDto dto, int currentUserId);
        Task<bool> ConvertToOnlineAsync(int activityId, int currentUserId);
        Task<FullTimelineActivityDto> UpdateTimelineActivityAsync(int activityId, int currentUserId, UpdateTimelineActivityDto dto);
        Task<bool> DeleteTimelineActivityAsync(int activityId, int currentUserId);
        //Task<IEnumerable<TimelineActivityInstanceDto>> GetTimelineForUserAsync(int userId, int daysAhead);
        Task<IEnumerable<TimelineActivityInstanceDto>> GetTimelineForUserAsync(int userId, DateTime from, DateTime to);
        Task<FullTimelineActivityDto?> AdjustTimelineAsync(ActivityModificationSuggestionDto dto, int userId);
        Task<FullTimelineActivityDto?> PlaceActivityAsync(int userId, ActivityPlacementDto dto);
        Task<FullTimelineActivityDto> UpdateTimelineActivityAutomaticAsync(int activityId, int currentUserId, UpdateTimelineActivityDto dto); //ONLY FOR ALGHORITM
    }
}
