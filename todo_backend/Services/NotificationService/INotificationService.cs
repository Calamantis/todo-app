using todo_backend.Dtos.Notification;

namespace todo_backend.Services.NotificationService
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto?>> GetAlertsAsync(int userId);
        Task<IEnumerable<NotificationDto?>> GetRemindersAsync(int userId);
        Task<NotificationDto?> GetAlertByIdAsync(int userId, int alertId);
        Task<NotificationDto?> GetReminderByIdAsync(int userId, int reminderId);
        Task<NotificationDto?> EditNotificationAsync(int userId, int notifId, EditNotificationDto notification);
        Task<NotificationDto> CreateNotificationAsync(int userId, CreateNotificationDto dto);
        Task<bool> DeleteNotificationAsync(int userId, int notifId);
    }
}
