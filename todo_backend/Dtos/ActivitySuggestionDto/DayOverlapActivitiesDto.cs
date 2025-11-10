namespace todo_backend.Dtos.ActivitySuggestionDto
{
    public class DayOverlapActivitiesDto
    {
            public DateTime Date { get; set; }
            public DateTime? SuggestedStart { get; set; }
            public DateTime? SuggestedEnd { get; set; }
            public int activityTime { get; set; }
            public int gapTime { get; set; }
            public List<ActivityBasicInfoDto> OverlappingActivities { get; set; } = new();
        public List<ActivityModificationSuggestionDto> ModificationSuggestions { get; set; } = new();

    }
}
