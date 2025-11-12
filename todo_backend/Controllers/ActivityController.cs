using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using todo_backend.Dtos.ActivityDto;
using todo_backend.Services.ActivityService;

namespace todo_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        // GET: api/activity/user/{userId}
        [HttpGet("user/get-activities")]
        public async Task<IActionResult> GetActivitiesByUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var activities = await _activityService.GetActivitiesByUserIdAsync(userId);
            if (activities == null || !activities.Any())
            {
                return NotFound("Nie znaleziono żadnych aktywności.");
            }

            return Ok(activities);
        }

        // GET: api/activity/{activityId}
        [HttpGet("get-activity-by-id")]
        public async Task<IActionResult> GetActivityById(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var activity = await _activityService.GetActivityByIdAsync(activityId, userId);

            if (activity == null)
            {
                return NotFound("Aktywność nie istnieje lub nie należy do tego użytkownika.");
            }

            return Ok(activity);
        }

        // POST: api/activity
        [HttpPost("create-activity")]
        public async Task<IActionResult> CreateActivity([FromBody] ActivityCreateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var createdActivity = await _activityService.CreateActivityAsync(dto, userId);

            if (createdActivity == null)
            {
                return BadRequest("Nie udało się utworzyć aktywności.");
            }

            return CreatedAtAction(nameof(GetActivityById), new { activityId = createdActivity.ActivityId }, createdActivity);
        }

        // PUT: api/activity/{activityId}
        [HttpPut("edit-activity")]
        public async Task<IActionResult> UpdateActivity(int activityId, [FromBody] UpdateActivityDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var updatedActivity = await _activityService.UpdateActivityAsync(activityId, dto, userId);

            if (updatedActivity == null)
            {
                return NotFound("Aktywność nie została znaleziona lub nie należy do tego użytkownika.");
            }

            return Ok(updatedActivity);
        }

        // DELETE: api/activity/{activityId}
        [HttpDelete("delete-activity")]
        public async Task<IActionResult> DeleteActivity(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _activityService.DeleteActivityAsync(activityId, userId);

            if (!result)
            {
                return NotFound("Aktywność nie została znaleziona lub nie należy do tego użytkownika.");
            }

            return NoContent(); // Sukces, brak treści
        }

    }
}
