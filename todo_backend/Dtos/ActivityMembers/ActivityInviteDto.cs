namespace todo_backend.Dtos.ActivityMembers
{
    public class ActivityInviteDto
    {
        public int ActivityId { get; set; }
        public int InvitedUserId { get; set; }
        public string ActivityTitle { get; set; } = string.Empty;
        public string OwnerFullName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
