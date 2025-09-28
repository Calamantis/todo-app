using System.ComponentModel.DataAnnotations;

namespace todo_backend.Dtos.User
{
    public class UpdateFullNameDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "Full name cannot be longer than 50 characters.")]
        public string FullName { get; set; } = string.Empty;
    }
}
