using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo_backend.Models
{
    public class TimelineRecurrenceInstance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InstanceId { get; set; }

        [ForeignKey(nameof(Activity))]
        public int ActivityId { get; set; }

        /// <summary>
        /// Data wystąpienia (dzień)
        /// </summary>
        [Required]
        public DateTime OccurrenceDate { get; set; }

        /// <summary>
        /// Czas rozpoczęcia (godzina w tym dniu)
        /// </summary>
        [Required]
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// Czas zakończenia (godzina w tym dniu)
        /// </summary>
        [Required]
        public TimeSpan EndTime { get; set; }

        /// <summary>
        /// Faktyczny czas trwania (w minutach)
        /// </summary>
        [Required]
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Flaga, czy wystąpienie zostało zrealizowane (np. do statystyk)
        /// </summary>
        public bool IsCompleted { get; set; } = false;

        // 🔗 Powiązanie z aktywnością
        public TimelineActivity? Activity { get; set; }
    }
}
