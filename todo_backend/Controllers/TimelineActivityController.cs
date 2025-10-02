using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using todo_backend.Data;
using todo_backend.Dtos.TimelineActivity;
using todo_backend.Models;
using todo_backend.Services.TimelineActivityService;

namespace todo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimelineActivityController : ControllerBase
    {
        private readonly ITimelineActivityService _timelineActivityService;

        public TimelineActivityController(ITimelineActivityService timelineActivityService)
        {
            _timelineActivityService = timelineActivityService;
        }

        //GET przeglądanie (swoich) aktywnosci
        [Authorize]
        [HttpGet("browse-activities")]
        public async Task<ActionResult<IEnumerable<FullTimelineActivityDto>>> GetActivities()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var activities = await _timelineActivityService.GetTimelineActivitiesAsync(userId);
            return Ok(activities);
        }

        //GET po id
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<FullTimelineActivityDto>> GetActivity(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var activity = await _timelineActivityService.GetTimelineActivityByIdAsync(activityId, userId);
            return Ok(activity);
        }
        //POST stworzenie aktywnosci
        [Authorize]
        [HttpPost("create-activity")]
        public async Task<ActionResult<FullTimelineActivityDto>> CreateActivity(CreateTimelineActivityDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var activity = await _timelineActivityService.CreateTimelineActivityAsync(dto, userId);
            return CreatedAtAction(nameof(GetActivity), new { id = activity.ActivityId }, activity);
            //return NoContent();
        }

        //PUT modyfikacja aktywności
        [Authorize]
        [HttpPut("edit-activity")]
        public async Task<ActionResult<FullTimelineActivityDto>> EditActivity(UpdateTimelineActivityDto dto, int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var activity = await _timelineActivityService.UpdateTimelineActivityAsync(activityId, userId, dto);
            return Ok(activity);
            //return NoContent();
        }

        //DELETE usunięcie aktywnosci
        [Authorize]
        [HttpDelete("delete-activity")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var success = await _timelineActivityService.DeleteTimelineActivityAsync(id, userId);

            if (!success)
                return NotFound();

            return NoContent();

        }
    }
}
