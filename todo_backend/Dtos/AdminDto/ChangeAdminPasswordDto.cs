using System.ComponentModel.DataAnnotations;

namespace todo_backend.Dtos.AdminDto
{
    public class ChangeAdminPasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
