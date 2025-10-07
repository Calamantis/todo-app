using Microsoft.EntityFrameworkCore;
using todo_backend.Data;
using todo_backend.Dtos.BlockedUsersDto;
using todo_backend.Models;

namespace todo_backend.Services.BlockedUsersService
{
    public class BlockedUsersService : IBlockedUsersService
    {
        private readonly AppDbContext _context;

        public BlockedUsersService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> BlockUserAsync(int userId, int blockedUserId)
        {
            if (userId == blockedUserId) return false;

            var exists = await _context.BlockedUsers
                .AnyAsync(b => b.UserId == userId && b.BlockedUserId == blockedUserId);
            if (exists) return false;

            var block = new BlockedUsers
            {
                UserId = userId,
                BlockedUserId = blockedUserId
            };

            _context.BlockedUsers.Add(block);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnblockUserAsync(int userId, int blockedUserId)
        {
            var block = await _context.BlockedUsers
                .FirstOrDefaultAsync(b => b.UserId == userId && b.BlockedUserId == blockedUserId);

            if (block == null) return false;

            _context.BlockedUsers.Remove(block);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsBlockedAsync(int userId, int targetUserId)
        {
            return await _context.BlockedUsers
                .AnyAsync(b => (b.UserId == userId && b.BlockedUserId == targetUserId) ||
                               (b.UserId == targetUserId && b.BlockedUserId == userId));
        }

        public async Task<IEnumerable<int>> GetBlockedUserIdsAsync(int userId)
        {
            return await _context.BlockedUsers
                .Where(b => b.UserId == userId)
                .Select(b => b.BlockedUserId)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlockedUsersDto?>> GetBlockedUsersAsync(int userId)
        {
            return await _context.BlockedUsers
                .Where(b => b.UserId == userId)
                .Include(b => b.BlockedUser) // do pobrania FullName
                .Select(b => new BlockedUsersDto
                {
                    BlockedUserId = b.BlockedUserId,
                    FullName = b.BlockedUser.FullName,
                    BlockedAt = b.BlockedAt
                })
                .ToListAsync();
        }

    }
}
