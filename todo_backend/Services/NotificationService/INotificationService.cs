using todo_backend.Dtos.Notification;

namespace todo_backend.Services.NotificationService
{
    public interface INotificationService
    {
        Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto, int userId);
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId);
        Task MarkAsReadAsync(int notificationId, int userId);
        Task DeleteNotificationAsync(int notificationId, int userId);
        Task<int> DeleteAllReadAsync(int userId);
    }
}
