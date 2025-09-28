using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using todo_backend.Dtos.Friendship;
using todo_backend.Dtos.User;
using todo_backend.Models;

namespace todo_backend.Services.UserFriendActions
{
    public interface IUserFriendActionsService
    {
        Task<IEnumerable<FriendshipDto>> GetUserFriendshipsAsync(int userId);
        Task<IEnumerable<FriendshipDto>> GetSentInvitesAsync(int userId);
        Task<IEnumerable<FriendshipDto>> GetReceivedInvitesAsync(int userId);
        Task<IEnumerable<UserResponseDto>> BrowseUsersAsync(int userId);
        Task<Friendship?> AddFriendshipAsync(int requesterId, int friendId);
        Task<bool> AcceptFriendshipAsync(int userId, int friendId);
        Task<bool> CancelFriendshipInviteAsync(int currentUserId, int friendId);
        Task<bool> DeleteFriendshipInviteAsync(int currentUserId, int requesterId);
        Task<bool> RemoveFriendAsync(int userId, int friendId);
    }
}
