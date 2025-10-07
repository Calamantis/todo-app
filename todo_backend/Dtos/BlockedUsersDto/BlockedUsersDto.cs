namespace todo_backend.Dtos.BlockedUsersDto
{
    public class BlockedUsersDto
    {
        public int BlockedUserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime BlockedAt { get; set; }
    }
}
