using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_backend.Data;
using todo_backend.Dtos;
using todo_backend.Models;
using todo_backend.Services.SecurityService;

namespace todo_backend.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordService _passwordService;

        public UserService(AppDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<IEnumerable<UserResponseDto>> GetUsersAsync()
        {
         var users = await _context.Users
            .Select(u => new  UserResponseDto
            {
                UserId = u.UserId,
                Email = u.Email,
                FullName = u.FullName
            })
            .ToListAsync();

            return users;
        }

        public async Task<UserResponseDto?> GetUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return null;

            var dto = new UserResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName
            };

            return dto;
        }

        public async Task<UserResponseDto> CreateUserAsync(UserCreateDto dto)
        {
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = _passwordService.Hash(dto.Password),
                FullName = dto.FullName
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var response = new UserResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName
            };

            return response;
        }

        public async Task<UserResponseDto?> UpdateUserAsync(int id, UserUpdateDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            user.FullName = dto.FullName;
            await _context.SaveChangesAsync();

            var response = new UserResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName
            };

            return response;

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


    }
}
