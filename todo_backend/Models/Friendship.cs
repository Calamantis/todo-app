using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo_backend.Models
{
    public class Friendship
    {
        [Key]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        [Key]
        [ForeignKey(nameof(Friend))]
        public int FriendId { get; set; }

        [Required]
        public DateTime FriendsSince { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "pending"; //pending, accepted, blocked

        // opcjonalnie nawigacje
        public User User { get; set; } = null!;
        public User Friend { get; set; } = null!;
    }
}
