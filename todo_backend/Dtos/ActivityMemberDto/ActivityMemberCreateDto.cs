using System.ComponentModel.DataAnnotations;

namespace todo_backend.Dtos.ActivityMemberDto
{
    public class ActivityMemberCreateDto
    {
        [Required]
        public int ActivityId { get; set; }

        [Required]
        public int UserId { get; set; }

        [MaxLength(20)]
        public string Role { get; set; } = "participant";

    }
}
