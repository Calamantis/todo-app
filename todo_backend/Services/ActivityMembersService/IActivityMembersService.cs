using todo_backend.Dtos.ActivityMembers;

namespace todo_backend.Services.ActivityMembersService
{
    public interface IActivityMembersService
    {
        Task<IEnumerable<FullActivityMembersDto?>> GetSentInvitesAsync(int userId);
        Task<IEnumerable<FullActivityMembersDto?>> GetAcceptedInvitesAsync(int userId);
        Task<bool> SendInviteAsync(int activityId, int userId, int invitedUserId);
        Task<bool> AcceptInviteAsync(int activityId, int userId);
        Task<bool> RevokeInviteAsync(int activityId, int userId);
    }
}
