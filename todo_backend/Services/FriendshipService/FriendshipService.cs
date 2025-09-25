using Microsoft.EntityFrameworkCore;
using todo_backend.Data;
using todo_backend.Dtos;
using todo_backend.Models;

namespace todo_backend.Services.FriendshipService
{
    public class FriendshipService : IFriendshipService
    {
        private readonly AppDbContext _context;

        public FriendshipService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Friendship?> AddFriendshipAsync(int requesterId, int friendId)
        {
            if (requesterId == friendId) return null;

            // normalizacja pary
            int userId = requesterId;
            if (userId > friendId)
                (userId, friendId) = (friendId, userId);

            // sprawdź czy istnieje
            var exists = await _context.Friendships
                .AnyAsync(f => f.UserId == userId && f.FriendId == friendId);

            if (exists) return null;

            var friendship = new Friendship
            {
                UserId = userId,
                FriendId = friendId,
                Status = "pending",
                FriendsSince = DateTime.UtcNow
            };

            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();

            return friendship;
        }

        public async Task<bool> AcceptFriendshipAsync(int userId, int friendId)
        {
            if (userId > friendId) (userId, friendId) = (friendId, userId);

            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);

            if (friendship == null) return false;

            friendship.Status = "accepted";
            friendship.FriendsSince = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<FriendshipResponseDto>> GetUserFriendshipsAsync(int userId)
        {
            var friends = await _context.Friendships
                .Where(f => f.Status == "accepted" && (f.UserId == userId || f.FriendId == userId))
                .Select(f => new FriendshipResponseDto
                {
                    UserId = f.UserId,
                    FriendId = f.FriendId,
                    FriendsSince = f.FriendsSince,
                    Status = f.Status,
                    // jeśli ja jestem UserId, to znajomym jest FriendId, w przeciwnym razie odwrotnie
                    FriendFullName = f.UserId == userId ? f.Friend.FullName : f.User.FullName,
                    FriendEmail = f.UserId == userId ? f.Friend.Email : f.User.Email
                })
                .ToListAsync();

            return friends;
        }

        public async Task<IEnumerable<FriendshipResponseDto>> GetSentInvitesAsync(int userId)
        {
            return await _context.Friendships
                .Where(f => f.UserId == userId && f.Status == "pending")
                .Join(_context.Users,
                      f => f.FriendId,
                      u => u.UserId,
                      (f, u) => new FriendshipResponseDto
                      {
                          UserId = f.UserId,
                          FriendId = f.FriendId,
                          FriendsSince = f.FriendsSince,
                          Status = f.Status,
                      })
                .ToListAsync();
        }

        public async Task<IEnumerable<FriendshipResponseDto>> GetReceivedInvitesAsync(int userId)
        {
            return await _context.Friendships
                .Where(f => f.FriendId == userId && f.Status == "pending")
                .Join(_context.Users,
                      f => f.UserId,
                      u => u.UserId,
                      (f, u) => new FriendshipResponseDto
                      {
                          UserId = f.UserId,
                          FriendId = f.FriendId,
                          FriendsSince = f.FriendsSince,
                          Status = f.Status,
                      })
                .ToListAsync();
        }

        public async Task<bool> RemoveFriendAsync(int userId, int friendId)
        {
            // normalizacja pary
            if (userId > friendId) (userId, friendId) = (friendId, userId);

            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId && f.Status == "accepted");

            if (friendship == null) return false;

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
