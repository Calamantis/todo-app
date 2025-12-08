using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using todo_backend.Dtos.Notification;
using todo_backend.Services.NotificationService;

namespace todo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [Authorize]
        [HttpPost("create-notification")]
        public async Task<IActionResult> Create(CreateNotificationDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var result = await _notificationService.CreateNotificationAsync(dto, userId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetForUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var result = await _notificationService.GetUserNotificationsAsync(userId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("{notificationId}/mark-as-read")]
        public async Task<IActionResult> MarkRead(int notificationId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            await _notificationService.MarkAsReadAsync(notificationId, userId);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> Delete(int notificationId, [FromQuery] int userId)
        {
            await _notificationService.DeleteNotificationAsync(notificationId, userId);
            return Ok();
        }

        [Authorize]
        [HttpDelete("delete-read")]
        public async Task<IActionResult> DeleteAllRead([FromQuery] int userId)
        {
            var deleted = await _notificationService.DeleteAllReadAsync(userId);
            return Ok(new { deleted });
        }

    }
}
