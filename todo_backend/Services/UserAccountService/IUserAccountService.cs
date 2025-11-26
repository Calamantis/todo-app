using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using todo_backend.Dtos.User;
using todo_backend.Models;

namespace todo_backend.Services.UserAccountService
{
    public interface IUserAccountService
    {
        Task<UserProfileResponseDto> GetUserDetailsAsync(int id);
        Task<UserProfileResponseDto> UpdateUserAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool?> ToggleAllowTimelineAsync(int id);
        Task<bool?> ToggleAllowFriendInvitesAsync(int id);
    }
}
