using todo_backend.Dtos;
using todo_backend.Models;

namespace todo_backend.Services.AuthService
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> AuthenticateAsync(LoginDto dto);
        string GenerateJwtToken(User user);
    }
}
