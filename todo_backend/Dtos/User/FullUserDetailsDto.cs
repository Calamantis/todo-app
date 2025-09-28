namespace todo_backend.Dtos.User
{
    public class FullUserDetailsDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool AllowMentions { get; set; }
        public bool AllowFriendInvites { get; set; }
    }
}
