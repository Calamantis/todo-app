namespace todo_backend.Dtos.ActivitySuggestionDto
{
    public class SuggestedTimelineActivityDto
    {
        public int ActivityId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int? SuggestedDurationMinutes { get; set; }
        public double Score { get; set; }
    }
}
