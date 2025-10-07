using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo_backend.Models
{
    public class BlockedUsers
    {
        [ForeignKey(nameof(User))]
        public int UserId { get; set; } // kto blokuje

        [ForeignKey(nameof(BlockedUser))]
        public int BlockedUserId { get; set; } // kto jest blokowany

        [Required]
        public DateTime BlockedAt { get; set; } = DateTime.UtcNow;

        // Nawigacje
        public User User { get; set; } = null!;          // właściciel blokady
        public User BlockedUser { get; set; } = null!;   // zablokowany
    }
}
