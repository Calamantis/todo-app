namespace todo_backend.Dtos.ActivityMemberDto
{
    public class ActivityMemberStatusUpdateDto
    {
        public int UserId { get; set; }
        public string Status { get; set; } // accepted / declined
    }
}
