namespace todo_backend.Dtos.TimelineRecurrenceInstanceDto
{
    public class TimelineRecurrenceInstanceResponseDto
    {
        public int InstanceId { get; set; }
        public int ActivityId { get; set; }
        public DateTime OccurrenceDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsCompleted { get; set; }
    }
}
