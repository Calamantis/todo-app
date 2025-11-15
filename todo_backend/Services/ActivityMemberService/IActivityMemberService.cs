using todo_backend.Dtos.ActivityMemberDto;

namespace todo_backend.Services.ActivityMembersService
{
    public interface IActivityMemberService
    {
        Task<IEnumerable<ActivityInviteDto>> GetOnlineActivitiesAsync(int userId);
        Task<IEnumerable<ActivityInviteDto>> GetReceivedInvitesAsync(int userId);
        Task<IEnumerable<ActivityInviteDto>> GetAcceptedMembersAsync(int activityId);
        Task<IEnumerable<ActivityInviteDto>> GetSentInvitesAsync(int activityId, int ownerId);
        Task<bool> SendInviteAsync(int ownerId, int activityId, int friendId);
        Task<bool> JoinByCodeAsync(int userId, string joinCode);
        Task<bool> UpdateInviteStatusAsync(int userId, int activityId, string newStatus);
        Task<bool> CancelInviteAsync(int ownerId, int activityId, int targetUserId);
        Task<bool> RemoveMemberAsync(int ownerId, int activityId, int targetUserId);
        Task<bool> LeaveActivityAsync(int userId, int activityId);
        Task<bool> RemoveAndBlockAsync(int ownerId, int activityId, int targetUserId);




        //Stare
        //Task<IEnumerable<ActivityInviteDto?>> GetSentInvitesAsync(int userId);
        //Task<IEnumerable<ActivityInviteDto?>> GetAcceptedInvitesAsync(int userId);
        //Task<IEnumerable<FullActivityMembersDto?>> GetSentInvitesAsync(int activityId, int userId);
        //Task<IEnumerable<FullActivityMembersDto?>> GetParticipantsOfActivityAsync(int activityId, int requestingUserId);
        //Task<ActivityInviteDto?> SendInviteAsync(int activityId, int userId, int invitedUserId);
        //Task<bool> JoinActivityByCodeAsync(string joinCode, int userId);
        //Task<bool> UpdateInviteStatusAsync(int activityId, int userId, string status);
        //Task<bool> RevokeInviteAsync(int activityId, int targetUserId, int ownerId);
        //Task<bool> RemoveParticipantAsync(int activityId, int targetUserId, int ownerId);
        //Task<bool> CancelInviteAsync(int activityId, int userId);
        //Task<bool> CancelActivityAsync(int activityId, int ownerId);
    }
}
