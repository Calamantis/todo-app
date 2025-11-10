namespace todo_backend.Dtos.TimelineRecurrenceInstanceDto
{
    public class TimelineRecurrenceInstanceCreateDto
    {
        public int ActivityId { get; set; }
        public DateTime OccurrenceDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsCompleted { get; set; } = false;
    }
}
