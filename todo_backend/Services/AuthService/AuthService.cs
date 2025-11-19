using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using todo_backend.Data;
using todo_backend.Dtos.Auth;
using todo_backend.Dtos.User;
using todo_backend.Models;
using todo_backend.Services.SecurityService;

namespace todo_backend.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordService _passwordService;
        private readonly IConfiguration _config;


        public AuthService(AppDbContext context, IPasswordService passwordService, IConfiguration config)
        {
            _context = context;
            _passwordService = passwordService;
            _config = config;
        }

        public async Task<UserResponseDto?> CreateUserAsync(UserCreateDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
               return null;

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = _passwordService.Hash(dto.Password),
                FullName = dto.FullName,
                Role = UserRole.User
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var response = new UserResponseDto
            {
                Email = user.Email,
                FullName = user.FullName
            };

            return response;
        }

        public async Task<AuthResponseDto?> AuthenticateAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return null;

            if (!_passwordService.Verify(user.PasswordHash, dto.Password))
                return null;

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Role = user.Role.ToString()//,
                //UserId = user.UserId,
                //Email = user.Email,
                //FullName = user.FullName,
                //AllowMentions = user.AllowMentions,
                //AllowFriendInvites = user.AllowFriendInvites
            };
        }

        public string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("fullName", user.FullName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
