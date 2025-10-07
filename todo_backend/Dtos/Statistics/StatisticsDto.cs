namespace todo_backend.Dtos.Statistics
{
    public class StatisticsDto
    {
        public string Category { get; set; } = string.Empty;
        public int TotalDuration { get; set; } // w minutach
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}
