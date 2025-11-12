namespace todo_backend.Dtos.ActivityDto
{
    public class UpdateActivityDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsRecurring { get; set; }
        public int? CategoryId { get; set; }
        public string? RecurrenceRule { get; set; }
    }
}
