namespace todo_backend.Dtos.TimelineActivity
{
    public class UpdateTimelineActivityDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsRecurring { get; set; }
        public string? RecurrenceRule { get; set; }
        public string? RecurrenceException { get; set; }
        public int PlannedDurationMinutes { get; set; }
    }
}
