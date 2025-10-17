using System.ComponentModel.DataAnnotations;

namespace todo_backend.Dtos.User
{
    public class UserResponseDto
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public string? BackgroundImageUrl { get; set; }
        public string? Synopsis { get; set; }
    }
}
