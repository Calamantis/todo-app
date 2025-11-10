namespace todo_backend.Dtos.TimelineRecurrenceExceptionDto
{
    public class TimelineRecurrenceExceptionResponseDto
    {
        public int ExceptionId { get; set; }
        public int ActivityId { get; set; }
        public DateTime ExceptionDate { get; set; }
        public TimeSpan? NewStartTime { get; set; }
        public int? NewDurationMinutes { get; set; }
        public bool IsSkipped { get; set; }
    }
}
