using System.ComponentModel.DataAnnotations;

namespace todo_backend.Dtos.TimelineActivity
{
    public class TimelineActivityDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;
    }
}
