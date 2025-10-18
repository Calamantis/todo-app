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

        //GET all alerts
        public async Task<IEnumerable<NotificationDto?>> GetAlertsAsync(int userId)
        {
            var ar = await _context.Notification
                .Where(ar => ar.IsAlert == true && ar.UserId == userId)
                .Select(ar => new NotificationDto
                {
                    NotificationId = ar.NotificationId,
                    Title = ar.Title,
                    Description = ar.Description,
                    RemindTime = ar.RemindTime,
                    IsRecurring = ar.IsRecurring,
                    RecurrenceRule = ar.RecurrenceRule
                }).ToListAsync();

            return ar;
        }

        //GET all user's reminders
        public async Task <IEnumerable<NotificationDto?>> GetRemindersAsync(int userId)
        {
            var ar = await _context.Notification
                .Where (ar => ar.IsAlert == false && ar.UserId == userId)
                .Select(ar => new NotificationDto
                {
                    NotificationId = ar.NotificationId,
                    Title = ar.Title,
                    Description = ar.Description,
                    RemindTime = ar.RemindTime,
                    IsRecurring = ar.IsRecurring,
                    RecurrenceRule = ar.RecurrenceRule
                }).ToListAsync();

            return ar;
        }

        //GET alert by id
        public async Task<NotificationDto?> GetAlertByIdAsync(int userId, int alertId)
        {
            var ar = await _context.Notification
                .Where(ar => ar.IsAlert == true && ar.NotificationId == alertId && ar.UserId == userId)
                .Select(ar => new NotificationDto
                {
                    NotificationId = ar.NotificationId,
                    Title = ar.Title,
                    Description = ar.Description,
                    RemindTime = ar.RemindTime,
                    IsRecurring = ar.IsRecurring,
                    RecurrenceRule = ar.RecurrenceRule
                }).FirstOrDefaultAsync();

            return ar;
        }

        //GET reminder by id
        public async Task<NotificationDto?> GetReminderByIdAsync(int userId, int reminderId)
        {
            var ar = await _context.Notification
                .Where(ar => ar.IsAlert == false && ar.NotificationId == reminderId && ar.UserId == userId)
                .Select(ar => new NotificationDto
                {
                    NotificationId = ar.NotificationId,
                    Title = ar.Title,
                    Description = ar.Description,
                    RemindTime = ar.RemindTime,
                    IsRecurring = ar.IsRecurring,
                    RecurrenceRule = ar.RecurrenceRule
                }).FirstOrDefaultAsync();

            return ar;
        }

        //PUT edit any of both
        public async Task<NotificationDto?> EditNotificationAsync(int userId,int notifId, EditNotificationDto notification)
        {
            var notif = await _context.Notification.FirstOrDefaultAsync(n => n.NotificationId == notifId && n.UserId == userId);
            if (notif == null) return null;

            notif.Title = notification.Title;
            notif.Description = notification.Description;
            notif.RemindTime = notification.RemindTime;
            notif.IsRecurring = notification.IsRecurring;
            notif.RecurrenceRule = notification.RecurrenceRule;

            await _context.SaveChangesAsync();

            return new NotificationDto
            {
                NotificationId = notif.NotificationId,
                Title = notif.Title,
                Description = notif.Description,
                RemindTime = notif.RemindTime,
                IsRecurring = notif.IsRecurring,
                RecurrenceRule = notif.RecurrenceRule
            };
        }

        //POST create any of both
        public async Task<NotificationDto> CreateNotificationAsync(int userId, CreateNotificationDto dto)
        {
            var entity = new Notification
            {
                UserId = userId,
                Title = dto.Title,
                Description = dto.Description,
                RemindTime = dto.RemindTime,
                IsRecurring = dto.IsRecurring,
                RecurrenceRule = dto.RecurrenceRule,
                IsAlert = dto.isAlert
            };

            _context.Notification.Add(entity);
            await _context.SaveChangesAsync();

            return new NotificationDto
            {
                NotificationId = entity.NotificationId,
                Title = entity.Title,
                Description = entity.Description,
                RemindTime = entity.RemindTime,
                IsRecurring = entity.IsRecurring,
                RecurrenceRule = entity.RecurrenceRule
            };
        }

        //DELETE delete any of both
        public async Task<bool> DeleteNotificationAsync(int userId, int notifId)
        {
            var notif = await _context.Notification.FirstOrDefaultAsync(n => n.UserId == userId && n.NotificationId == notifId);
            if (notif == null) return false;

            _context.Notification.Remove(notif);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
