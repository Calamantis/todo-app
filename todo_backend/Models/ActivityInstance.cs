using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo_backend.Models
{
    public class ActivityInstance
    {
        [Key] // Klucz główny
        public int InstanceId { get; set; }

        [Required] // Wymagane pole
        public int ActivityId { get; set; }

        // ForeignKey dla Activity
        [ForeignKey("ActivityId")]
        public Activity Activity { get; set; }

        public int? RecurrenceRuleId { get; set; }

        // ForeignKey dla ActivityRecurrenceRule
        [ForeignKey("RecurrenceRuleId")]
        public ActivityRecurrenceRule? RecurrenceRule { get; set; }

        [Required] // Wymagane pole
        public DateTime OccurrenceDate { get; set; }

        [Required] // Wymagane pole
        public TimeSpan StartTime { get; set; }

        [Required] // Wymagane pole
        public TimeSpan EndTime { get; set; }

        [Required] // Wymagane pole
        public int DurationMinutes { get; set; }

        [Required] // Wymagane pole
        public bool IsActive { get; set; } = true;

        [Required] // Wymagane pole
        public bool DidOccur { get; set; } = true;

        [Required] // Wymagane pole
        public bool IsException { get; set; } = false;
    }
}
