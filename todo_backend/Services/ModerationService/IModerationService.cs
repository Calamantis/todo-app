using todo_backend.Dtos.ModerationDto;

namespace todo_backend.Services.ModerationService
{
    public interface IModerationService
    {
        // UŻYTKOWNICY
        Task<IEnumerable<ModeratedUserDto>> GetAllUsersAsync();

        Task UpdateUserProfileImageAsync(int moderatorId, int userId, string url);
        Task ResetUserProfileImageAsync(int moderatorId, int userId);

        Task UpdateUserBackgroundImageAsync(int moderatorId, int userId, string url);
        Task ResetUserBackgroundImageAsync(int moderatorId, int userId);

        Task UpdateUserDisplayNameAsync(int moderatorId, int userId, string value);
        Task ResetUserDisplayNameAsync(int moderatorId, int userId);

        Task UpdateUserDescriptionAsync(int moderatorId, int userId, string value);
        Task ResetUserDescriptionAsync(int moderatorId, int userId);

        // AKTYWNOŚCI
        Task<IEnumerable<ModeratedActivityDto>> GetAllActivitiesAsync();

        Task UpdateActivityTitleAsync(int moderatorId, int activityId, string value);
        Task ResetActivityTitleAsync(int moderatorId, int activityId);

        Task UpdateActivityDescriptionAsync(int moderatorId, int activityId, string value);
        Task ResetActivityDescriptionAsync(int moderatorId, int activityId);

    }
}
