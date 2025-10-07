using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo_backend.Models
{
    public class ActivityStorage
    {
        [Key]
        public int TemplateId { get; set; }  // PK, auto increment

        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }      // właściciel template

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [ForeignKey(nameof(Category))]
        public int? CategoryId { get; set; }  // opcjonalna kategoria

        [Required]
        public bool IsRecurring { get; set; } = false;

        [MaxLength(255)]
        public string? RecurrenceRule { get; set; }  // np. DAILY, WEEKLY, cron

        // Nawigacje
        public User User { get; set; } = null!;             // właściciel template
        public Category? Category { get; set; } = null;     // kategoria, jeśli przypisana
    }
}
