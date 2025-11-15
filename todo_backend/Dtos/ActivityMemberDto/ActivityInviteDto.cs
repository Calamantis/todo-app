namespace todo_backend.Dtos.ActivityMemberDto
{
    public class ActivityInviteDto
    {
        public int ActivityId { get; set; }
        public string ActivityTitle { get; set; } = string.Empty;
        public int InvitedUserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // pending / accepted / declined / blocked
        public string Role {  get; set; } = string.Empty;
    }
}
