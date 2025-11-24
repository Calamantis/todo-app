namespace todo_backend.Dtos.BlockedUsersDto
{
    public class BlockedUsersDto
    {
        public string Email { get; set; } = string.Empty;
        public int BlockedUserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public string? BackgroundImageUrl { get; set; }
        public string? Synopsis { get; set; }
    }
}
