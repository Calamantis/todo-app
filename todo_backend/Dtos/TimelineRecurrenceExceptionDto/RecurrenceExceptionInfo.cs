namespace todo_backend.Dtos.TimelineRecurrenceExceptionDto
{
    public class RecurrenceExceptionInfo
    {
        public DateTime Date { get; init; }
        public List<TimeSpan> Times { get; init; } = new();
        public int? DurationMinutes { get; init; }
        public bool Skip { get; init; }
    }
}
