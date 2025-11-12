using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo_backend.Models
{
    public class ActivityRecurrenceRule
    {
        [Key] // Klucz główny
        public int RecurrenceRuleId { get; set; }

        [Required] // Wymagane pole
        public int ActivityId { get; set; }

        // ForeignKey dla Activity
        [ForeignKey("ActivityId")]
        public Activity Activity { get; set; }

        [Required] // Wymagane pole
        public string Type { get; set; } // DAY / WEEK / MONTH / YEAR

        public string? DaysOfWeek { get; set; } // Comma-separated e.g. MON,TUE
        public string? DaysOfMonth { get; set; } // Comma-separated e.g. 1,15,LAST

        public int? Interval { get; set; } // Occurrence every n days 

        [Required] // Wymagane pole
        public TimeSpan StartTime { get; set; }

        [Required] // Wymagane pole
        public TimeSpan EndTime { get; set; }

        [Required] // Wymagane pole
        public DateTime DateRangeStart { get; set; }

        public DateTime? DateRangeEnd { get; set; } // Optional

        [Required] // Wymagane pole
        public int DurationMinutes { get; set; }

        [Required] // Wymagane pole
        public bool IsActive { get; set; } = true;

        // Relacje
        public List<ActivityInstance> Instances { get; set; } = new List<ActivityInstance>();
    }
}
