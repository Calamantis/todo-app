using Microsoft.AspNetCore.Mvc;
using todo_backend.Dtos;
using todo_backend.Models;

namespace todo_backend.Services.FriendshipService
{
    public interface IFriendshipService
    {
        Task<Friendship?> AddFriendshipAsync(int requesterId, int friendId);
        Task<bool> AcceptFriendshipAsync(int userId, int friendId);
        //Task<IEnumerable<Friendship>> GetUserFriendshipsAsync(int userId);
        Task<IEnumerable<FriendshipResponseDto>> GetUserFriendshipsAsync(int userId);
        Task<IEnumerable<FriendshipResponseDto>> GetSentInvitesAsync(int userId);
        Task<IEnumerable<FriendshipResponseDto>> GetReceivedInvitesAsync(int userId);
        Task<bool> RemoveFriendAsync(int userId, int friendId);
    }
}
