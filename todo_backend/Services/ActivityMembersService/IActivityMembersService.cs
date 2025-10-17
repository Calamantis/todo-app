using todo_backend.Dtos.ActivityMembers;

namespace todo_backend.Services.ActivityMembersService
{
    public interface IActivityMembersService
    {
        Task<IEnumerable<ActivityInviteDto?>> GetSentInvitesAsync(int userId);
        Task<IEnumerable<ActivityInviteDto?>> GetAcceptedInvitesAsync(int userId);
        Task<IEnumerable<FullActivityMembersDto?>> GetSentInvitesAsync(int activityId, int userId);
        Task<IEnumerable<FullActivityMembersDto?>> GetParticipantsOfActivityAsync(int activityId, int requestingUserId);
        Task<ActivityInviteDto?> SendInviteAsync(int activityId, int userId, int invitedUserId);
        Task<bool> JoinActivityByCodeAsync(string joinCode, int userId);
        //Task<bool> AcceptInviteAsync(int activityId, int userId);
        Task<bool> UpdateInviteStatusAsync(int activityId, int userId, string status);
        //Task<bool> RevokeInviteAsync(int activityId, int userId);
        Task<bool> RevokeInviteAsync(int activityId, int targetUserId, int ownerId);
        Task<bool> RemoveParticipantAsync(int activityId, int targetUserId, int ownerId);
        Task<bool> CancelInviteAsync(int activityId, int userId);

        Task<bool> CancelActivityAsync(int activityId, int ownerId);
    }
}
