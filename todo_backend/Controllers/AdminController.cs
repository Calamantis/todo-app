using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using todo_backend.Dtos.AdminDto;
using todo_backend.Dtos.AuditLogDto;
using todo_backend.Services.AdminService;
using todo_backend.Services.AuditLogService;

namespace todo_backend.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IAuditLogService _auditLogService;

        public AdminController(IAdminService adminService, IAuditLogService auditLogService)
        {
            _adminService = adminService;
            _auditLogService = auditLogService;
        }

        // POST: api/admin/moderators (tworzenie nowego konta moderatora)
        [HttpPost("moderators")]
        public async Task<IActionResult> CreateModerator(AdminCreateModeratorRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int adminId = int.Parse(userIdClaim.Value);

            var id = await _adminService.CreateModeratorAccountAsync(
                adminId,
                request.Email,
                request.FullName,
                request.Password
            );

            return NoContent();
        }

        // POST: api/admin/users/{id}/promote-moderator (promocja już istniejącego konta)
        [HttpPost("users/{userId:int}/promote-moderator")]
        public async Task<IActionResult> PromoteToModerator(int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int adminId = int.Parse(userIdClaim.Value);

            await _adminService.PromoteToModeratorAsync(adminId, userId);
            return NoContent();
        }

        // DELETE: api/admin/users/{id} (usunięcie konta)
        [HttpDelete("users/{userId:int}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int adminId = int.Parse(userIdClaim.Value);

            await _adminService.DeleteUserAsync(adminId, userId);
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetLogs(
            [FromQuery] string? action,
            [FromQuery] string? entityType,
            [FromQuery] int? userId)
        {
            var logs = await _auditLogService.GetLogsAsync(action, entityType, userId);

            var dto = logs.Select(l => new AuditLogDto
            {
                LogId = l.LogId,
                UserId = l.UserId,
                Action = l.Action,
                EntityType = l.EntityType,
                EntityId = l.EntityId,
                Description = l.Description,
                CreatedAt = l.CreatedAt
            });

            return Ok(dto);
        }
    }
}
