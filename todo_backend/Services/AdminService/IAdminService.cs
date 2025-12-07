using todo_backend.Dtos.AdminDto;

namespace todo_backend.Services.AdminService
{
    public interface IAdminService
    {
        Task PromoteToModeratorAsync(int adminId, int targetUserId);
        Task<int> CreateModeratorAccountAsync(int adminId, string email, string fullName, string rawPassword);
        Task DeleteUserAsync(int adminId, int targetUserId);
    }
}
