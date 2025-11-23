namespace todo_backend.Dtos.Statistics
{
    public class StatisticsDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "Uncategorized";
        public int TotalDurationMinutes { get; set; }
        public int InstanceCount { get; set; } 
        public string ColorHex { get; set; } = string.Empty;
    }
}
