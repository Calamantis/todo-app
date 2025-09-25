using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo_backend.Models
{
    public class Friendship
    {
        public int UserId { get; set; }

        public int FriendId { get; set; }

        public DateTime FriendsSince { get; set; }

        // invitation status
        public string Status { get; set; } = "pending";

        // opcjonalnie nawigacje
        public User User { get; set; } = null!;
        public User Friend { get; set; } = null!;
    }
}
