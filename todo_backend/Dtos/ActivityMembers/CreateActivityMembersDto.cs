using System.ComponentModel.DataAnnotations;

namespace todo_backend.Dtos.ActivityMembers
{
    public class CreateActivityMembersDto
    {
        [Required]
        public int ActivityId { get; set; }

        [Required]
        public int UserId { get; set; }

        [MaxLength(20)]
        public string Role { get; set; } = "participant";

    }
}
