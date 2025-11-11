namespace todo_backend.Dtos.Recurrence
{
    public class InstanceDto
    {
        public int ActivityId { get; set; }
        public DateTime Start_time {  get; set; }
        public DateTime? End_time { get; set; }
        public bool Is_recurring { get; set; }
        public string Recurrence_rule { get; set; }
        public int PlannedDurationMinutes { get; set; }
    }
}
