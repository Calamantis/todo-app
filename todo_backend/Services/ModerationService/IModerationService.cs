using todo_backend.Dtos.ModerationDto;

namespace todo_backend.Services.ModerationService
{
    public interface IModerationService
    {
        // UŻYTKOWNICY
        Task<IEnumerable<ModeratedUserDto>> GetAllUsersAsync();

        Task UpdateUserProfileImageAsync(int userId, string url);
        Task ResetUserProfileImageAsync(int userId);

        Task UpdateUserBackgroundImageAsync(int userId, string url);
        Task ResetUserBackgroundImageAsync(int userId);

        Task UpdateUserDisplayNameAsync(int userId, string value);
        Task ResetUserDisplayNameAsync(int userId);

        Task UpdateUserDescriptionAsync(int userId, string value);
        Task ResetUserDescriptionAsync(int userId);

        // AKTYWNOŚCI
        Task<IEnumerable<ModeratedActivityDto>> GetAllActivitiesAsync();

        Task UpdateActivityTitleAsync(int activityId, string value);
        Task ResetActivityTitleAsync(int activityId);

        Task UpdateActivityDescriptionAsync(int activityId, string value);
        Task ResetActivityDescriptionAsync(int activityId);

    }
}
