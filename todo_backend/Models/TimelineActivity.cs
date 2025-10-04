using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace todo_backend.Models
{
    public class TimelineActivity
    {
        [Key]
        public int ActivityId { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public int OwnerId { get; set; }

        [Required]
        [MaxLength(100)]
        [MinLength(1)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }


        public int? CategoryId { get; set; }  //jest z ? po to aby przy okazji usuwania kategorii można było usunąć tylko kategorie bez aktywności.
        public Category? Category { get; set; } = null!;

        [Required]
        public DateTime Start_time { get; set; }

        public DateTime? End_time { get; set; }

        [Required]
        public bool Is_recurring { get; set; }

        public string? Recurrence_rule { get; set; }


        public User User { get; set; } = null!;
    }
}
