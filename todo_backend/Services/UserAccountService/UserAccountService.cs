using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using todo_backend.Data;
using todo_backend.Dtos.User;
using todo_backend.Models;
using todo_backend.Services.SecurityService;

namespace todo_backend.Services.UserAccountService
{
    public class UserAccountService : IUserAccountService
    {
        private readonly AppDbContext _context;

        public UserAccountService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserResponseDto?> GetUserDetailsAsync(int id)
        {
            var entity = await _context.Users.FindAsync(id);
            if (entity == null) return null;

            return new UserResponseDto
            {
                Email = entity.Email,
                FullName = entity.FullName,
                ProfileImageUrl = entity.ProfileImageUrl,
                BackgroundImageUrl = entity.BackgroundImageUrl,
                Synopsis = entity.Synopsis
            };
        }

        public async Task<UserResponseDto> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null!;

            user.FullName = dto.FullName;
            user.ProfileImageUrl = dto.ProfileImageUrl;
            user.BackgroundImageUrl = dto.BackgroundImageUrl;
            user.Synopsis = dto.Synopsis;
            await _context.SaveChangesAsync();

            return new UserResponseDto
            {
                Email = user.Email,
                FullName = user.FullName,
                ProfileImageUrl = user.ProfileImageUrl,
                BackgroundImageUrl = user.BackgroundImageUrl,
                Synopsis = user.Synopsis
            };
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users
            .Include(u => u.TimelineActivities)
            .Include(u => u.Categories)
            .FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return false;


            // Usuń wszystkich znajomych i z ich znajomych
            var friendships = _context.Friendships
            .Where(f => f.UserId == id || f.FriendId == id);
            _context.Friendships.RemoveRange(friendships);

            // Usuń wszystkie aktywności
            _context.TimelineActivities.RemoveRange(user.TimelineActivities);

            //Usuń wszystkie templatki aktywnosci
            _context.ActivityStorage.RemoveRange(user.ActivityStorage);

            // Usuń wszystkie kategorie
            _context.Categories.RemoveRange(user.Categories);

            //Usuń wszystkich zablokowanych użytkowników
            _context.BlockedUsers.RemoveRange(user.BlockedUsers);

            //Usuń statystyki uzytkownika
            _context.Statistics.RemoveRange(user.Statistics);

            // Usuń wszystkie zaproszenia, które użytkownik wysłał (ActivityMembers - Role: owner)
            var sentInvites = _context.ActivityMembers.Where(am => am.UserId == id);
            _context.ActivityMembers.RemoveRange(sentInvites);

            // Usuń zaproszenia, które użytkownik otrzymał
            var receivedInvites = _context.ActivityMembers.Where(am => am.UserId == id && am.Status == "pending");
            _context.ActivityMembers.RemoveRange(receivedInvites);


            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool?> ToggleAllowMentionsAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            user.AllowMentions = !user.AllowMentions;
            await _context.SaveChangesAsync();

            return user.AllowMentions;
        }

        public async Task<bool?> ToggleAllowFriendInvitesAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            user.AllowFriendInvites = !user.AllowFriendInvites;
            await _context.SaveChangesAsync();

            return user.AllowFriendInvites;
        }
    }
}
