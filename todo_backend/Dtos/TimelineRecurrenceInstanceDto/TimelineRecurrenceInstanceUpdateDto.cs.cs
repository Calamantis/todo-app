namespace todo_backend.Dtos.TimelineRecurrenceInstanceDto
{
    public class TimelineRecurrenceInstanceUpdateDto
    {
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? DurationMinutes { get; set; }
        public bool? IsCompleted { get; set; }
    }
}
