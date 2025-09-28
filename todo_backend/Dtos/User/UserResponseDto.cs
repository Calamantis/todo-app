using System.ComponentModel.DataAnnotations;

namespace todo_backend.Dtos.User
{
    public class UserResponseDto
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
