namespace todo_backend.Dtos.InstanceExclusionDto
{
    public class InstanceExclusionResponseDto
    {
        public int ExclusionId { get; set; }
        public int UserId { get; set; }
        public int ActivityId { get; set; }

        public DateTime ExcludedDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public bool IsActive { get; set; }
    }
}
