using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using todo_backend.Dtos.User;
using todo_backend.Models;

namespace todo_backend.Services.UserAccountService
{
    public interface IUserAccountService
    {
        Task<UserResponseDto?> GetUserDetailsAsync(int id);
        Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool?> ToggleAllowMentionsAsync(int id);
        Task<bool?> ToggleAllowFriendInvitesAsync(int id);
    }
}
