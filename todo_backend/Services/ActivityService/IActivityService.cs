using todo_backend.Dtos.ActivityDto;

namespace todo_backend.Services.ActivityService
{
    public interface IActivityService
    {
        Task<IEnumerable<ActivityResponseDto>> GetActivitiesByUserIdAsync(int currentUserId);
        Task<ActivityResponseDto?> GetActivityByIdAsync(int activityId, int currentUserId);
        Task<ActivityResponseDto?> CreateActivityAsync(ActivityCreateDto dto, int currentUserId);
        Task<ActivityResponseDto?> UpdateActivityAsync(int activityId, UpdateActivityDto dto, int currentUserId);
        Task<bool> DeleteActivityAsync(int activityId, int currentUserId);

        Task<bool> ConvertToOnlineAsync(int activityId, int currentUserId);
        Task<bool> ConvertToFriendsOnlyAsync(int activityId, int currentUserId);
        Task<bool> ConvertToOfflineAsync(int activityId, int currentUserId);
    }
}
