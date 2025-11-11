using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using todo_backend.Data;
using todo_backend.Dtos.ActivitySuggestionDto;
using todo_backend.Dtos.TimelineActivity;
using todo_backend.Dtos.TimelineRecurrenceInstanceDto;
using todo_backend.Models;
using todo_backend.Services.TimelineActivityService;
using todo_backend.Services.TimelineRecurrenceExceptionService;
using todo_backend.Services.TimelineRecurrenceInstanceService;

namespace todo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimelineActivityController : ControllerBase
    {
        private readonly ITimelineActivityService _timelineActivityService;
        private readonly ITimelineRecurrenceInstanceService _recurrenceInstanceService;

        public TimelineActivityController(ITimelineActivityService timelineActivityService, ITimelineRecurrenceInstanceService recurrenceInstanceService)
        {
            _timelineActivityService = timelineActivityService;
            _recurrenceInstanceService = recurrenceInstanceService;
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
        [HttpGet("find-activity")]
        public async Task<ActionResult<FullTimelineActivityDto>> GetActivity(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var activity = await _timelineActivityService.GetTimelineActivityByIdAsync(activityId, userId);
            if (activity == null) return BadRequest();
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
            if (activity == null) return BadRequest("errormessage");
            return CreatedAtAction(nameof(GetActivity), new { id = activity.ActivityId }, activity);
            //return NoContent();
        }

        //PATCH ustaw aktywność na PUBLICZNĄ
        [Authorize]
        [HttpPatch("convert-activity-to-online")]
        public async Task<IActionResult> ConvertToOnline(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var result = await _timelineActivityService.ConvertToOnlineAsync(activityId, userId);
            if (!result)
                return BadRequest("Activity not found or not owned by user.");

            return Ok(new { message = "Activity converted to online successfully." });
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

        [Authorize]
        [HttpPost("repeat/{activityId}")]
        public async Task<IActionResult> RepeatActivity(int activityId, [FromBody] DateTime newStart)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var activity = await _timelineActivityService.GetTimelineActivityByIdAsync(activityId, userId);
            if (activity == null)
                return NotFound();

            var dto = new TimelineRecurrenceInstanceCreateDto
            {
                ActivityId = activityId,
                OccurrenceDate = newStart.Date,
                StartTime = newStart.TimeOfDay,
                EndTime = newStart.AddMinutes(activity.PlannedDurationMinutes).TimeOfDay,
                DurationMinutes = activity.PlannedDurationMinutes,
                IsCompleted = false
            };

            var result = await _recurrenceInstanceService.CreateAsync(dto);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-timeline")]
        public async Task<IActionResult> GetTimeline([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);


            var activities = await _timelineActivityService.GetTimelineForUserAsync(userId, from, to);
            return Ok(activities);
        }

        //[Authorize]
        //[HttpPut("adjust-timeline")]
        //public async Task<ActionResult<FullTimelineActivityDto>> AdjustTimeline(ActivityModificationSuggestionDto dto)
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //    if (userIdClaim == null) return Unauthorized();
        //    int userId = int.Parse(userIdClaim.Value);

        //    var updated = await _timelineActivityService.AdjustTimelineAsync(dto, userId);
        //    if (updated == null) return NotFound();

        //    return Ok(updated);
        //}

        //[Authorize]
        //[HttpPut("place-activity")]
        //public async Task<ActionResult<FullTimelineActivityDto>> PlaceActivity([FromBody] ActivityPlacementDto dto)
        //{
        //    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        //    var result = await _timelineActivityService.PlaceActivityAsync(userId, dto);

        //    if (result == null)
        //        return NotFound("Activity not found.");

        //    return Ok(result);
        //}



    }
}
