namespace todo_backend.Dtos.Notification
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? VisibleFrom { get; set; }
    }
}
