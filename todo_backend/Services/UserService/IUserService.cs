using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using todo_backend.Dtos;
using todo_backend.Models;

namespace todo_backend.Services.UserService
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetUsersAsync();
        Task<UserResponseDto?> GetUserAsync(int id);
        Task<UserResponseDto> CreateUserAsync(UserCreateDto dto);
        Task<UserResponseDto?> UpdateUserAsync(int id, UserUpdateDto dto);
        Task<bool> DeleteUserAsync(int id);
    }
}
