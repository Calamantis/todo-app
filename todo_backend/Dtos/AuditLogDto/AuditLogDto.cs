namespace todo_backend.Dtos.AuditLogDto
{
    public class AuditLogDto
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; } = "";
        public string EntityType { get; set; } = "";
        public int? EntityId { get; set; }
        public string Description { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
