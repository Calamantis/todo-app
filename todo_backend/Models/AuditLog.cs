using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace todo_backend.Models
{
    public class AuditLog
    {
        [Key]
        public int LogId { get; set; }

        // Kto wykonał akcję
        public int UserId { get; set; }

        // Jaki użytkownik jej dokonał
        public User User { get; set; }

        // Nazwa akcji, np. DELETE_CATEGORY, EDIT_ACTIVITY
        public string Action { get; set; } = string.Empty;

        // Typ encji, np. "Category", "Activity", "Instance", "User"
        public string EntityType { get; set; } = string.Empty;

        // Id encji, np. CategoryId = 1002
        public int? EntityId { get; set; }

        // Szczegółowy opis
        public string Description { get; set; } = string.Empty;

        // Czas utworzenia logu
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
