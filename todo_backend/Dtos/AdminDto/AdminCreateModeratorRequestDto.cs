using todo_backend.Models;

namespace todo_backend.Dtos.AdminDto
{
    public class AdminCreateModeratorRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
