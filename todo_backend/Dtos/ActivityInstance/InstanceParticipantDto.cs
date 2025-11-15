namespace todo_backend.Dtos.ActivityInstance
{
    public class InstanceParticipantDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "owner" / "participant"
        public bool IsAttending { get; set; }
    }
}
