namespace todo_backend.Dtos.ActivityMembers
{
    public class FullActivityMembersDto
    {
        public int ActivityId { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        // Dodatkowe info o użytkowniku (opcjonalnie)
        public string? UserFullName { get; set; }
        public string? UserEmail { get; set; }
    }
}
