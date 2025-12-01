namespace todo_backend.Dtos.ModerationDto
{
    public class ModeratedUserDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;

        public string? DisplayName { get; set; }
        public string? Description { get; set; }

        public string? ProfileImageUrl { get; set; }
        public string? BackgroundImageUrl { get; set; }

        public string Role { get; set; } = string.Empty;
    }
}
