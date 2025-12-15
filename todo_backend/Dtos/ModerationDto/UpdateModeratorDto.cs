using System.ComponentModel.DataAnnotations;

namespace todo_backend.Dtos.ModerationDto
{
    public class UpdateModeratorDto
    {
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? FullName { get; set; }

        // opcjonalna zmiana hasła
        [MinLength(6)]
        public string? NewPassword { get; set; }
    }
}
