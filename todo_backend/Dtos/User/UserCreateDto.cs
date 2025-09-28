using System.ComponentModel.DataAnnotations;

namespace todo_backend.Dtos.User
{
    public class UserCreateDto
    {
        [Required]
        [EmailAddress]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [MaxLength(255, ErrorMessage = "Password cannot be longer than 255 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(50, ErrorMessage = "Full name cannot be longer than 50 characters.")]
        public string FullName { get; set; } = string.Empty;
    }
}
