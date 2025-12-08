namespace todo_backend.Dtos.Notification
{
    public class CreateNotificationDto
    {
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime? VisibleFrom { get; set; }
    }
}
