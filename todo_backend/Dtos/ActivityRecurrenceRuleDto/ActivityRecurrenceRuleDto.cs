namespace todo_backend.Dtos.ActivityRecurrenceRuleDto
{
    public class ActivityRecurrenceRuleDto
    {
        public int RecurrenceRuleId { get; set; }
        public int ActivityId { get; set; }
        public string Type { get; set; } // DAY / WEEK / MONTH / YEAR
        public string? DaysOfWeek { get; set; } // Comma-separated e.g. MON,TUE
        public string? DaysOfMonth { get; set; } // Comma-separated e.g. 1,15,LAST
        public DateTime? DayOfYear { get; set; }
        public int? Interval { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime DateRangeStart { get; set; }
        public DateTime? DateRangeEnd { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
