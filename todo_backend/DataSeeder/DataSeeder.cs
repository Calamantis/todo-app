using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using todo_backend.Data;
using todo_backend.Models;
using todo_backend.Services.SecurityService;

namespace todo_backend.Services
{
    public class DataSeeder
    {
        private readonly AppDbContext _context;
        private readonly IPasswordService _passwordService;

        public DataSeeder(AppDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();

            await SeedSystemCategoriesAsync();
            await SeedAdminAccountAsync();
        }

        private async Task SeedSystemCategoriesAsync()
        {
            if (await _context.Categories.AnyAsync(c => c.IsSystem))
                return;

            var categories = new List<Category>
            {
                new Category { Name = "Praca", ColorHex = "#f97316", IsSystem = true },
                new Category { Name = "Nauka", ColorHex = "#3b82f6", IsSystem = true },
                new Category { Name = "Sport", ColorHex = "#22c55e", IsSystem = true },
                new Category { Name = "Relaks", ColorHex = "#a855f7", IsSystem = true },
                new Category { Name = "Zdrowie", ColorHex = "#14b8a6", IsSystem = true },
                new Category { Name = "Rozrywka", ColorHex = "#ef4444", IsSystem = true },
                new Category { Name = "Inne", ColorHex = "#94a3b8", IsSystem = true }
            };

            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();
        }

        private async Task SeedAdminAccountAsync()
        {
            // Jeśli admin już istnieje — nie rób nic
            if (await _context.Users.AnyAsync(u => u.Role == UserRole.Admin))
                return;

            var admin = new User
            {
                Email = "admin@example.com",
                FullName = "System Administrator",
                Role = UserRole.Admin,
                ProfileImageUrl = "/TempImageTests/DefaultProfileImage.png",
                BackgroundImageUrl = "/TempImageTests/DefaultBgImage.jpg",
                PasswordHash = _passwordService.Hash("admin123!@#.")
            };

            await _context.Users.AddAsync(admin);
            await _context.SaveChangesAsync();
        }

    }
}
