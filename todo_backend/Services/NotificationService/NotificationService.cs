using todo_backend.Data;
using todo_backend.Dtos.Notification;
using Microsoft.EntityFrameworkCore;
using todo_backend.Models;

namespace todo_backend.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception($"User with id {userId} not found");

            var notif = new Notification
            {
                UserId = userId,
                Title = dto.Title,
                Message = dto.Message,
                VisibleFrom = dto.VisibleFrom,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Notification.Add(notif);
            await _context.SaveChangesAsync();

            return new NotificationDto
            {
                NotificationId = notif.NotificationId,
                Title = notif.Title,
                Message = notif.Message,
                IsRead = notif.IsRead,
                VisibleFrom = notif.VisibleFrom,
                CreatedAt = notif.CreatedAt
            };
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId)
        {
            return await _context.Notification
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    NotificationId = n.NotificationId,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    VisibleFrom = n.VisibleFrom,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int notificationId, int userId)
        {
            var notif = await _context.Notification
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

            if (notif == null)
                throw new Exception("Notification not found or access denied");

            notif.IsRead = true;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(int notificationId, int userId)
        {
            var notif = await _context.Notification
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

            if (notif == null)
                throw new Exception("Notification not found or access denied");

            _context.Notification.Remove(notif);
            await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAllReadAsync(int userId)
        {
            var readNotifications = await _context.Notification
                .Where(n => n.UserId == userId && n.IsRead)
                .ToListAsync();

            if (!readNotifications.Any())
                return 0;

            _context.Notification.RemoveRange(readNotifications);
            await _context.SaveChangesAsync();

            return readNotifications.Count;
        }

    }
}
