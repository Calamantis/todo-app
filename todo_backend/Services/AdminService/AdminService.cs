using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using todo_backend.Data;
using todo_backend.Dtos.AdminDto;
using todo_backend.Dtos.ModerationDto;
using todo_backend.Models;
using todo_backend.Services.AuditLogService;
using todo_backend.Services.SecurityService;

namespace todo_backend.Services.AdminService
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordService _passwordService;
        private readonly IAuditLogService _logger;

        public AdminService(AppDbContext context, IPasswordService passwordService, IAuditLogService logger)
        {
            _context = context;
            _passwordService = passwordService;
            _logger = logger;
        }

        public async Task PromoteToModeratorAsync(int adminId, int targetUserId)
        {
            var user = await _context.Users.FindAsync(targetUserId)
                ?? throw new KeyNotFoundException("User not found.");

            if (user.Role == UserRole.Admin)
                throw new InvalidOperationException("Cannot change role of an admin.");

            user.Role = UserRole.Moderator;
            await _context.SaveChangesAsync();
            await _logger.LogAsync(adminId, "PROMOTE_MODERATOR", "Moderator", targetUserId, "Promoted user to moderator");
        }

        public async Task<int> CreateModeratorAccountAsync(int adminId, string email, string fullName, string rawPassword)
        {
            // bardzo uproszczona walidacja:
            if (await _context.Users.AnyAsync(u => u.Email == email))
                throw new InvalidOperationException("User with this email already exists.");

            var user = new User
            {
                Email = email,
                FullName = fullName,
                Role = UserRole.Moderator,
                ProfileImageUrl = "\\TempImageTests\\DefaultProfileImage.png",
                BackgroundImageUrl = "\\TempImageTests\\DefaultBgImage.jpg"
            };

            user.PasswordHash = _passwordService.Hash(rawPassword);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user.UserId;
        }

        public async Task DeleteUserAsync(int adminId, int targetUserId)
        {
            var user = await _context.Users.FindAsync(targetUserId)
                ?? throw new KeyNotFoundException("User not found.");

            if (user.Role == UserRole.Admin)
                throw new InvalidOperationException("Cannot delete admin account.");

            // tu możesz zrobić dodatkowo:
            // - usunięcie aktywności, instancji, itp. albo flagę IsActive = false

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            await _logger.LogAsync(adminId, "DELETE_USER", "User", targetUserId, "Permanently deleted users account");

        }

        public async Task DeleteActivityAsync(int adminId, int activityId)
        {
            // 1️⃣ Usuń uczestników aktywności (ActivityMembers)
            var members = await _context.ActivityMembers
                .Where(m => m.ActivityId == activityId)
                .ToListAsync();

            Console.WriteLine($"[DELETE_ACTIVITY] Members count: {members.Count}");

            if (members.Any())
                _context.ActivityMembers.RemoveRange(members);

            // 2️⃣ Usuń instancje aktywności
            var instances = await _context.ActivityInstances
                .Where(i => i.ActivityId == activityId)
                .ToListAsync();

            Console.WriteLine($"[DELETE_ACTIVITY] Instances count: {instances.Count}");

            if (instances.Any())
                _context.ActivityInstances.RemoveRange(instances);

            // 3️⃣ Usuń samą aktywność
            var activity = await _context.Activities.FindAsync(activityId)
                ?? throw new KeyNotFoundException("Activity not found.");

            _context.Activities.Remove(activity);

            // 4️⃣ Zapisz wszystko w jednej transakcji
            await _context.SaveChangesAsync();

            // 5️⃣ Audit log
            await _logger.LogAsync(
                adminId,
                "DELETE_ACTIVITY",
                "Activity",
                activityId,
                "Permanently deleted activity with members and instances"
            );
        }




        public async Task<IEnumerable<ModeratorDto>> GetModeratorsAsync()
        {
            return await _context.Users
                .Where(u => u.Role == UserRole.Moderator)
                .Select(u => new ModeratorDto
                {
                    UserId = u.UserId,
                    Email = u.Email,
                    FullName = u.FullName
                })
                .OrderBy(u => u.Email)
                .ToListAsync();
        }

        public async Task DeleteModeratorAsync(int adminId, int moderatorId)
        {
            var user = await _context.Users.FindAsync(moderatorId)
                ?? throw new KeyNotFoundException("User not found.");

            if (user.Role != UserRole.Moderator)
                throw new InvalidOperationException("User is not a moderator.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            await _logger.LogAsync(
                adminId,
                "DELETE_MODERATOR",
                "User",
                moderatorId,
                "Permanently deleted moderator account"
            );
        }
    }
}
