using System.ComponentModel.DataAnnotations;

namespace todo_backend.Dtos.ActivitySuggestionDto
{
    public class ActivityPlacementSuggestionDto
    {
        public int ActivityId { get; set; }
        public TimeSpan? PreferredStart { get; set; }
        public TimeSpan? PreferredEnd { get; set; }
        public List<DayOfWeek>? PreferredDays { get; set; }
        public DateTime? StartDate { get; set; } 
        public DateTime? EndDate { get; set; }

        //[RegularExpression("^(standard|efficient)$", ErrorMessage = "Mode must be 'standard' or 'efficient'.")]
        //public string Mode { get; set; } = "standard";
    }
}
