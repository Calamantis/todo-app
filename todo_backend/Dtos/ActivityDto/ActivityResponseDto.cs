namespace todo_backend.Dtos.ActivityDto
{
    public class ActivityResponseDto
    {
        public int ActivityId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsRecurring { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }  // Zawiera nazwę kategorii, jeśli jest przypisana
        public string? JoinCode { get; set; }
    }
}
