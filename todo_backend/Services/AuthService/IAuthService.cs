using todo_backend.Dtos.Auth;
using todo_backend.Dtos.User;
using todo_backend.Models;

namespace todo_backend.Services.AuthService
{
    public interface IAuthService
    {
        Task<UserResponseDto?> CreateUserAsync(UserCreateDto dto);
        Task<AuthResponseDto?> AuthenticateAsync(LoginDto dto);
        string GenerateJwtToken(User user);
    }
}
