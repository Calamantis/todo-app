namespace todo_backend.Dtos.InstanceExclusionDto
{
    public class InstanceExclusionUpdateDto
    {
        public DateTime ExcludedDate { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public bool IsActive { get; set; }
    }
}
