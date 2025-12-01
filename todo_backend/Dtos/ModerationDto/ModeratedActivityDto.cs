namespace todo_backend.Dtos.ModerationDto
{
    public class ModeratedActivityDto
    {
        public int ActivityId { get; set; }
        public int OwnerId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public string OwnerEmail { get; set; } = string.Empty;

        public int InstancesCount { get; set; }

        public bool IsOnline { get; set; }

        public bool IsRecurring { get; set; }
    }
}
