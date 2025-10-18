namespace todo_backend.Dtos.Notification
{
    public class CreateNotificationDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime RemindTime { get; set; }
        public bool IsRecurring { get; set; }
        public string? RecurrenceRule { get; set; }
        public bool isAlert { get; set; } = false;
    }
}
