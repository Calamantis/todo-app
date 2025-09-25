namespace todo_backend.Dtos
{
    public class FriendshipResponseDto
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }
        public DateTime FriendsSince { get; set; }
        public string Status { get; set; } = "pending";

        // uproszczone dane znajomego
        public string FriendFullName { get; set; } = string.Empty;
        public string FriendEmail { get; set; } = string.Empty;
    }
}
