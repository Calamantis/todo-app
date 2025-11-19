using todo_backend.Dtos.AdminDto;

namespace todo_backend.Services.AdminService
{
    public interface IAdminService
    {
        Task PromoteToModeratorAsync(int targetUserId);
        Task<int> CreateModeratorAccountAsync(string email, string fullName, string rawPassword);
        Task DeleteUserAsync(int targetUserId);
    }
}
