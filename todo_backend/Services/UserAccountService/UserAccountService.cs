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
                FullName = entity.FullName
            };
        }

        public async Task<UserResponseDto> UpdateUserAsync(int id, UpdateFullNameDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null!;

            user.FullName = dto.FullName;
            await _context.SaveChangesAsync();

            return new UserResponseDto
            {
                Email = user.Email,
                FullName = user.FullName
            };
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            var friendships = _context.Friendships
            .Where(f => f.UserId == id || f.FriendId == id);
            _context.Friendships.RemoveRange(friendships);

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
