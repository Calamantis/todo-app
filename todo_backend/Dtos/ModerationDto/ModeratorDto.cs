namespace todo_backend.Dtos.ModerationDto
{
    public class ModeratorDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
    }
}
