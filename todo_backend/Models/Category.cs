using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo_backend.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(User))]
        public int? UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(7)]
        public string? ColorHex { get; set; }

        public bool IsSystem { get; set; } = false;

        public User User { get; set; } = null!;
        public ICollection<TimelineActivity> TimelineActivities { get; set; } = new List<TimelineActivity>();
        //public ICollection<ActivityStorage> ActivityStorage { get; set; } = new List<ActivityStorage>();

    }
}