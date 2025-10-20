namespace todo_backend.Dtos.TimelineActivity
{
    public class TimelineActivityInstanceDto
    {
        public int ActivityId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsRecurring { get; set; }
        public string ColorHex { get; set; }
    }
}
