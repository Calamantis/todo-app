using todo_backend.Models;

namespace todo_backend.Services.AuditLogService
{
    public interface IAuditLogService
    {
       Task LogAsync(int userId, string action, string entityType, int? entityId = null, string? description = null);

        Task<IEnumerable<AuditLog>> GetLogsAsync(string? action = null, string? entityType = null, int? userId = null);
    }
}
