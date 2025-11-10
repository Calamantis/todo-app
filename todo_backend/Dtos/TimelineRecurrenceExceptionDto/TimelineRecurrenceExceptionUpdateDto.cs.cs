namespace todo_backend.Dtos.TimelineRecurrenceExceptionDto
{
    public class TimelineRecurrenceExceptionUpdateDto
    {
        public TimeSpan? NewStartTime { get; set; }
        public int? NewDurationMinutes { get; set; }
        public bool IsSkipped { get; set; }
    }
}
