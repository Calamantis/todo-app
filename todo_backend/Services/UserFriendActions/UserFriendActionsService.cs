using Microsoft.EntityFrameworkCore;
using todo_backend.Data;
using todo_backend.Dtos.Friendship;
using todo_backend.Dtos.User;
using todo_backend.Models;
using todo_backend.Services.BlockedUsersService;

namespace todo_backend.Services.UserFriendActions
{
    public class UserFriendActionsService : IUserFriendActionsService
    {
        private readonly AppDbContext _context;
        private readonly IBlockedUsersService _blockedUsersService;

        public UserFriendActionsService(AppDbContext context, IBlockedUsersService blockedUsersService)
        {
            _context = context;
            _blockedUsersService = blockedUsersService;
        }

        //GET user's friendships
        public async Task<IEnumerable<FriendshipDto>> GetUserFriendshipsAsync(int userId)
        {
            var friends = await _context.Friendships
                .Where(f => f.Status == "accepted" && (f.UserId == userId || f.FriendId == userId))
                .Select(f => new FriendshipDto
                {
                    FriendId = f.UserId == userId ? f.FriendId : f.UserId,
                    FriendsSince = f.FriendsSince,
                    // jeśli ja jestem UserId, to znajomym jest FriendId, w przeciwnym razie odwrotnie
                    FriendFullName = f.UserId == userId ? f.Friend.FullName : f.User.FullName,
                    FriendEmail = f.UserId == userId ? f.Friend.Email : f.User.Email,
                    FriendImage = f.UserId == userId ? f.Friend.ProfileImageUrl : f.User.ProfileImageUrl,
                    FriendBackground = f.UserId == userId ? f.Friend.BackgroundImageUrl : f.User.BackgroundImageUrl,
                    Synopsis = f.UserId == userId ? f.Friend.Synopsis : f.User.Synopsis
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
                          FriendId = f.FriendId,
                          FriendsSince = f.FriendsSince,
                          FriendFullName = f.UserId == userId ? f.Friend.FullName : f.User.FullName,
                          FriendEmail = f.UserId == userId ? f.Friend.Email : f.User.Email,
                          FriendImage = f.UserId == userId ? f.Friend.ProfileImageUrl : f.User.ProfileImageUrl,
                          FriendBackground = f.UserId == userId ? f.Friend.BackgroundImageUrl : f.User.BackgroundImageUrl,
                          Synopsis = f.UserId == userId ? f.Friend.Synopsis : f.User.Synopsis
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
                          FriendId = f.UserId,
                          FriendsSince = f.FriendsSince,
                          FriendFullName = f.UserId == userId ? f.Friend.FullName : f.User.FullName,
                          FriendEmail = f.UserId == userId ? f.Friend.Email : f.User.Email,
                          FriendImage = f.UserId == userId ? f.Friend.ProfileImageUrl : f.User.ProfileImageUrl,
                          FriendBackground = f.UserId == userId ? f.Friend.BackgroundImageUrl : f.User.BackgroundImageUrl,
                          Synopsis = f.UserId == userId ? f.Friend.Synopsis : f.User.Synopsis
                      })
                .ToListAsync();
        }

        //GET users -> browse users (to send friend request to)
        public async Task<IEnumerable<UserResponseDto>> BrowseUsersAsync(int userId)
        {
            // 1) Pobierz ID znajomych
            var friendIds = await _context.Friendships
                .Where(f => f.UserId == userId || f.FriendId == userId)
                .Select(f => f.UserId == userId ? f.FriendId : f.UserId)
                .ToListAsync();

            // 2) Pobierz zablokowanych przeze mnie
            var iBlockedIds = await _context.BlockedUsers
                .Where(b => b.UserId == userId)
                .Select(b => b.BlockedUserId)
                .ToListAsync();

            // 3) Pobierz tych, którzy zablokowali mnie
            var blockedMeIds = await _context.BlockedUsers
                .Where(b => b.BlockedUserId == userId)
                .Select(b => b.UserId)
                .ToListAsync();

            var Admins = await _context.Users
                .Where(a => a.Role == UserRole.Admin || a.Role == UserRole.Moderator)
                .Select(a => a.UserId)
                .ToListAsync();

            // 4) Połącz wszystkie ID wykluczeń
            var excluded = friendIds
                .Concat(iBlockedIds)
                .Concat(blockedMeIds)
                .Concat(Admins)
                .ToHashSet();

            var users = await _context.Users
                   .Where(u =>
                       u.AllowFriendInvites &&
                       u.UserId != userId &&
                       !excluded.Contains(u.UserId)
                   )
                   .Select(u => new UserResponseDto
                   {
                       UserId = u.UserId,
                       Email = u.Email,
                       FullName = u.FullName,
                       ProfileImageUrl = u.ProfileImageUrl,
                       BackgroundImageUrl = u.BackgroundImageUrl,
                       Synopsis = u.Synopsis
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

            var exists = await _context.Friendships
                .AnyAsync(f => (f.UserId == requesterId && f.FriendId == friendId) ||
                               (f.UserId == friendId && f.FriendId == requesterId));
            if (exists) return null;

            var isBlocked = await _blockedUsersService.IsBlockedAsync(requesterId, friendId);
            if (isBlocked) return null; 

            var friendship = new Friendship
            {
                UserId = requesterId,
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
            var friendship = await _context.Friendships
                    .FirstOrDefaultAsync(f =>
                        f.UserId == friendId && f.FriendId == userId &&
                        f.Status == "pending"); // akceptacja tylko pending

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
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f =>
                    ((f.UserId == userId && f.FriendId == friendId) ||
                     (f.UserId == friendId && f.FriendId == userId)) &&
                     f.Status == "accepted");

            if (friendship == null) return false;

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
