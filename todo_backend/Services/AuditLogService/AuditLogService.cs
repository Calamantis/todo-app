using todo_backend.Data;
using todo_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace todo_backend.Services.AuditLogService
{
    public class AuditLogService : IAuditLogService
    {
        public readonly AppDbContext _context;

        public AuditLogService(AppDbContext context) { _context = context; }

        public async Task LogAsync(
            int userId,
            string action,
            string entityType,
            int? entityId = null,
            string? description = null)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Description = description ?? "",
                CreatedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetLogsAsync(
            string? action = null,
            string? entityType = null,
            int? userId = null)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(action))
                query = query.Where(l => l.Action == action);

            if (!string.IsNullOrWhiteSpace(entityType))
                query = query.Where(l => l.EntityType == entityType);

            if (userId.HasValue)
                query = query.Where(l => l.UserId == userId.Value);

            return await query
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }
    }
}
