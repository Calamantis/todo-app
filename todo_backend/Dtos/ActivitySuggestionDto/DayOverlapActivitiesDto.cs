namespace todo_backend.Dtos.ActivitySuggestionDto
{
    public class DayOverlapActivitiesDto
    {
            public DateTime Date { get; set; }
            public DateTime SuggestedStart { get; set; }
            public DateTime SuggestedEnd { get; set; }
            public int ActivityTime { get; set; }
            public int GapTime { get; set; }
            public List<ActivityBasicInfoDto> OverlappingActivities { get; set; } = new();
    }
}
