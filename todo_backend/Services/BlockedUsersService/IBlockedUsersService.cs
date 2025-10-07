using todo_backend.Dtos.BlockedUsersDto;

namespace todo_backend.Services.BlockedUsersService
{
    public interface IBlockedUsersService
    {
        Task<bool> BlockUserAsync(int userId, int blockedUserId);
        Task<bool> UnblockUserAsync(int userId, int blockedUserId);
        Task<bool> IsBlockedAsync(int userId, int targetUserId); // czy targetUser jest zablokowany przez userId
        Task<IEnumerable<int>> GetBlockedUserIdsAsync(int userId);
        Task<IEnumerable<BlockedUsersDto?>> GetBlockedUsersAsync(int userId);
    }
}
