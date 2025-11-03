using System.Text.Json.Serialization;

namespace todo_backend.Dtos.ActivitySuggestionDto
{
    public class ActivitySuggestionDto
    {
        public int? CategoryId { get; set; } // opcjonalna kategoria
        public int? PlannedDurationMinutes { get; set; } // np. 45
        public TimeSpan? PreferredStart { get; set; } // np. 08:00
        public TimeSpan? PreferredEnd { get; set; }   // np. 12:00

        public List<DayOfWeek>? PreferredDays { get; set; } // np. [Monday, Wednesday, Friday]
    }
}
