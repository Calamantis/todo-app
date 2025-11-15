using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo_backend.Models
{
    public class InstanceExclusion
    {
        [Key]
        public int ExclusionId { get; set; }

        [Required]
        public int ActivityId { get; set; }

        [ForeignKey("ActivityId")]
        public Activity Activity { get; set; }

        /// <summary>
        /// Użytkownik wykluczający dane wystąpienie (działa dla online i offline).
        /// </summary>
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// Dzień wystąpienia aktywności, który ma być pominięty (bez czasu).
        /// </summary>
        [Required]
        public DateTime ExcludedDate { get; set; }

        /// <summary>
        /// Start konkretnego wystąpienia, które użytkownik pomija.
        /// </summary>
        [Required]
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// Koniec konkretnego wystąpienia.
        /// </summary>
        [Required]
        public TimeSpan EndTime { get; set; }

        /// <summary>
        /// Gdyby użytkownik chciał cofnąć wykluczenie.
        /// </summary>
        [Required]
        public bool IsActive { get; set; } = true;
    }
}
