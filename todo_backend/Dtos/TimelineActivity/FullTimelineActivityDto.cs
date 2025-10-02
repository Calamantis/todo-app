using System.ComponentModel.DataAnnotations;
using todo_backend.Models;

namespace todo_backend.Dtos.TimelineActivity
{
    public class FullTimelineActivityDto
    {
        public int ActivityId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsRecurring { get; set; }
        public string? RecurrenceRule { get; set; }
        public string? CategoryName { get; set; }
    }
}
