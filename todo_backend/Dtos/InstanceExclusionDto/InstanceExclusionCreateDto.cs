namespace todo_backend.Dtos.InstanceExclusionDto
{
    public class InstanceExclusionCreateDto
    {
        public int UserId { get; set; }
        public int ActivityId { get; set; }

        public DateTime ExcludedDate { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
