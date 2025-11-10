namespace todo_backend.Dtos.ActivitySuggestionDto
{
    public class ActivityPlacementDto
    {
        public int ActivityId { get; set; }
        public DateTime SuggestedStart { get; set; }
        public int DurationMinutes { get; set; }
        public int Mode { get; set; } // 1=Rule, 2=Exception, 3=Single
    }
}
