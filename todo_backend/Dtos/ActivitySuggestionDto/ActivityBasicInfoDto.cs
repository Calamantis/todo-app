namespace todo_backend.Dtos.ActivitySuggestionDto
{
    public class ActivityBasicInfoDto
    {
        public int ActivityId { get; set; }
        public string? Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
