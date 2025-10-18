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

        //GET all reminders
        [Authorize]
        [HttpGet("my-reminders")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetReminders()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var reminders = await _notificationService.GetRemindersAsync(userId);
            return Ok(reminders);
        }

        //GET all alerts
        [Authorize]
        [HttpGet("my-alerts")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAlerts()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var alerts = await _notificationService.GetAlertsAsync(userId);
            return Ok(alerts);
        }

        //GET alert by id
        [Authorize]
        [HttpGet("find-alert-by-id")]
        public async Task<ActionResult<NotificationDto>> GetAlertById(int alertId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var alert = await _notificationService.GetAlertByIdAsync(userId, alertId);
            if (alert == null) return NotFound();
            return Ok(alert);
        }

        //GET reminder by id
        [Authorize]
        [HttpGet("find-reminder-by-id")]
        public async Task<ActionResult<NotificationDto>> GetReminderById(int reminderId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var reminder = await _notificationService.GetReminderByIdAsync(userId, reminderId);
            if (reminder == null) return NotFound();
            return Ok(reminder);
        }

        //PUT edit any of both
        [Authorize]
        [HttpPut("edit-notification")]
        public async Task<ActionResult<NotificationDto>> EditNotification(int notifId,[FromBody] EditNotificationDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var notif = await _notificationService.EditNotificationAsync(userId, notifId, dto);
            if (notif == null) return NotFound();
            return Ok(notif);

        }

        //POST create any of both
        [Authorize]
        [HttpPost("create-notification")]
        public async Task<ActionResult<NotificationDto>> CreateNotification([FromBody] CreateNotificationDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var notif = await _notificationService.CreateNotificationAsync(userId, dto);
            if (notif == null) return NotFound();
            return Ok(notif);
        }

        //DELETE delete any of both
        [Authorize]
        [HttpDelete("delete-notification")]
        public async Task<IActionResult> DeleteNotification(int notifId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var notif = await _notificationService.DeleteNotificationAsync(userId, notifId);
            if (!notif) return NotFound();

            return NoContent();
        }

    }
}
