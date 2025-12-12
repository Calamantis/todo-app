using Azure.Core;
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

        public async Task<UserProfileResponseDto> GetUserDetailsAsync(int id)
        {
            var entity = await _context.Users.FindAsync(id);
            if (entity == null) return null;

            // Tworzymy pełną ścieżkę do obrazka
            var profileImageUrl = string.IsNullOrEmpty(entity.ProfileImageUrl)
                ? null
                : $"/{id}/{id}_profile.jpg";
            var backgroundImageUrl = string.IsNullOrEmpty(entity.BackgroundImageUrl)
                ? null
                : $"/{id}/{id}_bg.jpg";

            return new UserProfileResponseDto
            {
                Email = entity.Email,
                FullName = entity.FullName,
                ProfileImageUrl = entity.ProfileImageUrl,
                BackgroundImageUrl = entity.BackgroundImageUrl,
                Synopsis = entity.Synopsis,
                AllowFriendInvites = entity.AllowFriendInvites,
                AllowDataStatistics = entity.AllowDataStatistics
            };
        }


        //public async Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto dto)
        //{
        //    var user = await _context.Users.FindAsync(id);
        //    if (user == null) return null;

        //    user.FullName = dto.FullName;
        //    user.ProfileImageUrl = dto.ProfileImageUrl;
        //    user.BackgroundImageUrl = dto.BackgroundImageUrl;
        //    user.Synopsis = dto.Synopsis;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        Console.WriteLine($"Database update failed: {ex.InnerException?.Message}");
        //        return null;
        //    }

        //    return new UserResponseDto
        //    {
        //        Email = user.Email,
        //        FullName = user.FullName,
        //        ProfileImageUrl = user.ProfileImageUrl,
        //        BackgroundImageUrl = user.BackgroundImageUrl,
        //        Synopsis = user.Synopsis
        //    };
        //}


        public async Task<UserProfileResponseDto> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            // Ścieżki do plików
            string profileImagePath = null;
            string backgroundImagePath = null;

            // Sprawdzamy, czy plik profilowy jest dostarczony i zapisujemy go
            if (dto.ProfileImage != null)
            {
                var userFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", id.ToString());
                if (!Directory.Exists(userFolder))
                {
                    Directory.CreateDirectory(userFolder); // Tworzymy folder dla użytkownika, jeśli nie istnieje
                }

                // Generujemy nazwę pliku
                profileImagePath = Path.Combine(userFolder, $"{id}_profile.jpg");

                // Zapisujemy plik
                using (var stream = new FileStream(profileImagePath, FileMode.Create))
                {
                    await dto.ProfileImage.CopyToAsync(stream); // Zapisujemy plik na dysku
                }

                user.ProfileImageUrl = $"/{id}/{id}_profile.jpg"; // Ustawiamy pełną ścieżkę w bazie
            }

            // Sprawdzamy, czy plik tła jest dostarczony i zapisujemy go
            if (dto.BackgroundImage != null)
            {
                var userFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", id.ToString());
                if (!Directory.Exists(userFolder))
                {
                    Directory.CreateDirectory(userFolder); // Tworzymy folder, jeśli nie istnieje
                }

                // Generujemy nazwę pliku
                backgroundImagePath = Path.Combine(userFolder, $"{id}_bg.jpg");

                // Zapisujemy plik
                using (var stream = new FileStream(backgroundImagePath, FileMode.Create))
                {
                    await dto.BackgroundImage.CopyToAsync(stream); // Zapisujemy plik na dysku
                }

                user.BackgroundImageUrl = $"/{id}/{id}_bg.jpg"; // Ustawiamy pełną ścieżkę w bazie
            }

            // Aktualizacja pozostałych danych
            user.Email = dto.Email;
            user.FullName = dto.FullName;
            user.Synopsis = dto.Synopsis;
            user.AllowFriendInvites = dto.AllowFriendInvites;
            user.AllowDataStatistics = dto.AllowDataStatistics;

            try
            {
                await _context.SaveChangesAsync(); // Zapisujemy zmiany w bazie danych
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database update failed: {ex.InnerException?.Message}");
                return null;
            }

            return new UserProfileResponseDto
            {
                Email = user.Email,
                FullName = user.FullName,
                ProfileImageUrl = user.ProfileImageUrl,
                BackgroundImageUrl = user.BackgroundImageUrl,
                Synopsis = user.Synopsis,
                AllowFriendInvites = user.AllowFriendInvites,
                AllowDataStatistics = user.AllowDataStatistics
            };
        }



        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users
            //.Include(u => u.TimelineActivities)
            .Include(u => u.Categories)
            .FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return false;


            // Usuń wszystkich znajomych i z ich znajomych
            var friendships = _context.Friendships
            .Where(f => f.UserId == id || f.FriendId == id);
            _context.Friendships.RemoveRange(friendships);

            // Usuń wszystkie aktywności
            //_context.TimelineActivities.RemoveRange(user.TimelineActivities);

            //Usuń wszystkie templatki aktywnosci
           // _context.ActivityStorage.RemoveRange(user.ActivityStorage);

            // Usuń wszystkie kategorie
            _context.Categories.RemoveRange(user.Categories);

            //Usuń wszystkich zablokowanych użytkowników
            _context.BlockedUsers.RemoveRange(user.BlockedUsers);

            //Usuń statystyki uzytkownika
            _context.Statistics.RemoveRange(user.Statistics);

            // Usuń wszystkie alerty i remindery użytkownika
            _context.Notification.RemoveRange(user.Notifications);

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

        public async Task<bool?> ToggleAllowTimelineAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            user.AllowTimeline = !user.AllowTimeline;
            await _context.SaveChangesAsync();

            return user.AllowTimeline;
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
