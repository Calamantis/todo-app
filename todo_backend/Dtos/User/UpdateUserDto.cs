using System.ComponentModel.DataAnnotations;

namespace todo_backend.Dtos.User
{
    public class UpdateUserDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "Full name cannot be longer than 50 characters.")]
        public string FullName { get; set; } = string.Empty;

        public string? ProfileImageUrl { get; set; }
        public string? BackgroundImageUrl { get; set; }
        public string? Synopsis { get; set; }

    }
}
