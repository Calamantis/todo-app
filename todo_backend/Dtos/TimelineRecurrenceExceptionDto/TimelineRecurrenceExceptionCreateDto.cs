using todo_backend.Models;

namespace todo_backend.Dtos.TimelineRecurrenceExceptionDto
{
    public class TimelineRecurrenceExceptionCreateDto
    {
        public int ActivityId { get; set; }
        public DateTime ExceptionDate { get; set; }
        public TimeSpan? NewStartTime { get; set; }
        public int? NewDurationMinutes { get; set; }
        public RecurrenceExceptionMode Mode { get; set; }
    }
}
