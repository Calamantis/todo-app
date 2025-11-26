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

        public bool isFriendsOnly { get; set; } = false;

        public string? JoinCode { get; set; } = null;

        // Relacje
        public List<ActivityRecurrenceRule> RecurrenceRules { get; set; } = new List<ActivityRecurrenceRule>();
        public List<ActivityInstance> Instances { get; set; } = new List<ActivityInstance>();
        public ICollection<InstanceExclusion> InstanceExclusions { get; set; } = new List<InstanceExclusion>();
        public ICollection<ActivityMember> ActivityMembers { get; set; } = new List<ActivityMember>();
    }
}