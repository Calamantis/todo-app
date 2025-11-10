namespace todo_backend.Dtos.ActivitySuggestionDto
{
    public class ActivityModificationSuggestionDto
    {
        public int ActivityId { get; set; }
        public string ModificationType { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime NewStartTime { get; set; }
        public DateTime? NewEndTime { get; set; }
    }
}
