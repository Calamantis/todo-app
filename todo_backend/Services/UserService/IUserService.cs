using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using todo_backend.Dtos.User;
using todo_backend.Models;

namespace todo_backend.Services.UserService
{
    public interface IUserService
    {
        Task<IEnumerable<FullUserDetailsDto>> GetUsersAsync();
        Task<FullUserDetailsDto?> GetUserAsync(int id);
        //Task<UserResponseDto> CreateUserAsync(UserCreateDto dto);
        Task<FullUserDetailsDto?> UpdateUserAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(int id);
    }
}