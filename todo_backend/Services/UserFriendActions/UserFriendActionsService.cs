using Microsoft.EntityFrameworkCore;
using todo_backend.Data;
using todo_backend.Dtos.Friendship;
using todo_backend.Dtos.User;
using todo_backend.Models;

namespace todo_backend.Services.UserFriendActions
{
    public class UserFriendActionsService : IUserFriendActionsService
    {
        private readonly AppDbContext _context;

        public UserFriendActionsService(AppDbContext context)
        {
            _context = context;
        }

        //GET user's friendships
        public async Task<IEnumerable<FriendshipDto>> GetUserFriendshipsAsync(int userId)
        {
            var friends = await _context.Friendships
                .Where(f => f.Status == "accepted" && (f.UserId == userId || f.FriendId == userId))
                .Select(f => new FriendshipDto
                {
                    FriendsSince = f.FriendsSince,
                    // jeśli ja jestem UserId, to znajomym jest FriendId, w przeciwnym razie odwrotnie
                    FriendFullName = f.UserId == userId ? f.Friend.FullName : f.User.FullName,
                    FriendEmail = f.UserId == userId ? f.Friend.Email : f.User.Email
                })
                .ToListAsync();

            return friends;
        }

        //GET user's sent friend requests
        public async Task<IEnumerable<FriendshipDto>> GetSentInvitesAsync(int userId)
        {
            return await _context.Friendships
                .Where(f => f.UserId == userId && f.Status == "pending")
                .Join(_context.Users,
                      f => f.FriendId,
                      u => u.UserId,
                      (f, u) => new FriendshipDto
                      {
                          FriendsSince = f.FriendsSince,
                          FriendFullName = f.UserId == userId ? f.Friend.FullName : f.User.FullName,
                          FriendEmail = f.UserId == userId ? f.Friend.Email : f.User.Email
                      })
                .ToListAsync();
        }

        //GET user's recieved friend requests
        public async Task<IEnumerable<FriendshipDto>> GetReceivedInvitesAsync(int userId)
        {
            return await _context.Friendships
                .Where(f => f.FriendId == userId && f.Status == "pending")
                .Join(_context.Users,
                      f => f.UserId,
                      u => u.UserId,
                      (f, u) => new FriendshipDto
                      {
                          FriendsSince = f.FriendsSince,
                          FriendFullName = f.UserId == userId ? f.Friend.FullName : f.User.FullName,
                          FriendEmail = f.UserId == userId ? f.Friend.Email : f.User.Email
                      })
                .ToListAsync();
        }

        //GET users -> browse users (to send friend request to)
        public async Task<IEnumerable<UserResponseDto>> BrowseUsersAsync(int userId)
        {
            var users = await _context.Users
               .Where(u => u.AllowFriendInvites && u.UserId != userId)
               .Select(u => new UserResponseDto
               {
                   Email = u.Email,
                   FullName = u.FullName
               })
               .ToListAsync();

            return users;
        }

        //POST send invite
        public async Task<Friendship?> AddFriendshipAsync(int requesterId, int friendId)
        {
            if (requesterId == friendId) return null;

            //czy uzytkownik pozwala wgl na zaproszenia
            var targetUser = await _context.Users.FindAsync(friendId);
            if (targetUser == null || !targetUser.AllowFriendInvites)
                return null;

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


        //PATCH accept recieved friend request
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

        //DELETE cancel send invite
        public async Task<bool> CancelFriendshipInviteAsync(int currentUserId, int friendId)
        {
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => f.UserId == currentUserId && f.FriendId == friendId && f.Status == "pending");

            if (friendship == null)
                return false;

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();

            return true;
        }

        //DELETE decline friend invite
        public async Task<bool> DeleteFriendshipInviteAsync(int currentUserId, int requesterId)
        {
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => f.UserId == requesterId && f.FriendId == currentUserId && f.Status == "pending");

            if (friendship == null)
                return false;

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();

            return true;
        }


        //DELETE delete friend from friendlist
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
