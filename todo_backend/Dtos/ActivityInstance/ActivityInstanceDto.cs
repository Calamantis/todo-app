namespace todo_backend.Dtos.ActivityInstance
{
    public class ActivityInstanceDto
    {
        public int InstanceId { get; set; }
        public DateTime OccurrenceDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsActive { get; set; }
        public bool DidOccur { get; set; }
        public bool IsException { get; set; }

        // Relacje
        public int ActivityId { get; set; }
        public int? RecurrenceRuleId { get; set; }
    }
}
