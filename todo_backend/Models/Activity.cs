//using Microsoft.EntityFrameworkCore;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Diagnostics.CodeAnalysis;

//namespace todo_backend.Models
//{
//    public class TimelineActivity
//    {
//        [Key]
//        public int ActivityId { get; set; }

//        [Required]
//        [ForeignKey(nameof(User))]
//        public int OwnerId { get; set; }

//        [Required]
//        [MaxLength(100)]
//        [MinLength(1)]
//        public string Title { get; set; } = string.Empty;

//        [MaxLength(500)]
//        public string? Description { get; set; }


//        public int? CategoryId { get; set; }  //jest z ? po to aby przy okazji usuwania kategorii można było usunąć tylko kategorie bez aktywności.
//        public Category? Category { get; set; } = null!;

//        [Required]
//        public DateTime Start_time { get; set; }

//        public DateTime? End_time { get; set; }

//        public int PlannedDurationMinutes { get; set; }

//        public bool IsManuallyFinished { get; set; }

//        [Required]
//        public bool Is_recurring { get; set; }

//        public string? Recurrence_rule { get; set; }

//        public bool IsActive { get; set; } = true;

//        [MaxLength(12)]
//        public string? JoinCode { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();

//        public ICollection<ActivityMembers> ActivityMembers { get; set; } = new List<ActivityMembers>();
//        public User User { get; set; } = null!;
//    }
//}


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo_backend.Models
{
    public class Activity
    {
        [Key] // Klucz główny
        public int ActivityId { get; set; }

        [Required] // Wymagane pole
        public int OwnerId { get; set; }

        // ForeignKey dla User
        [ForeignKey("OwnerId")]
        public User Owner { get; set; } = null!;

        // ForeignKey dla Category
        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [Required] // Wymagane pole
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required] // Wymagane pole
        public bool IsRecurring { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public string? JoinCode { get; set; } = null;

        // Relacje
        public List<ActivityRecurrenceRule> RecurrenceRules { get; set; } = new List<ActivityRecurrenceRule>();
        public List<ActivityInstance> Instances { get; set; } = new List<ActivityInstance>();
    }
}