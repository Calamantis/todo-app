using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using todo_backend.Dtos.ActivityInstance;
using todo_backend.Services.ActivityInstanceService;

namespace todo_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityInstanceController : ControllerBase
    {
        private readonly IActivityInstanceService _service;

        public ActivityInstanceController(IActivityInstanceService service)
        {
            _service = service;
        }

        // GET: api/activityinstance/user/{userId}
        [HttpGet("user/get-instances")]
        public async Task<IActionResult> GetAllInstancesByUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var instances = await _service.GetAllInstancesByUserIdAsync(userId);
            return Ok(instances);
        }

        // GET: api/activityinstance/activity/{activityId}
        [HttpGet("activity/get-activity-instances")]
        public async Task<IActionResult> GetInstancesByActivityId(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var instances = await _service.GetInstancesByActivityIdAsync(activityId, userId);
            if (instances == null)
            {
                return NotFound("Instancje nie należą do tego użytkownika.");
            }
            return Ok(instances);
        }

        //// GET: api/activityinstance/daterange
        //[HttpGet("daterange")]
        //public async Task<IActionResult> GetInstancesByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int userId)
        //{
        //    var instances = await _service.GetInstancesByDateRangeAsync(startDate, endDate, userId);
        //    return Ok(instances);
        //}

        // POST: api/activityinstance
        [HttpPost("create-instance")]
        public async Task<IActionResult> CreateInstance([FromBody] ActivityInstanceDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var instance = await _service.CreateInstanceAsync(dto, userId);
            if (instance == null)
            {
                return BadRequest("Nie udało się utworzyć instancji.");
            }
            return CreatedAtAction(nameof(GetInstancesByActivityId), new { activityId = instance.ActivityId }, instance);
        }

        // PUT: api/activityinstance/{instanceId}
        [HttpPut("edit-instance")]
        public async Task<IActionResult> UpdateInstance(int instanceId, [FromBody] ActivityInstanceDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var instance = await _service.UpdateInstanceAsync(instanceId, dto, userId);
            if (instance == null)
            {
                return NotFound("Instancja nie została znaleziona lub nie należy do tego użytkownika.");
            }
            return Ok(instance);
        }

        // DELETE: api/activityinstance/{instanceId}
        [HttpDelete("delete-instance")]
        public async Task<IActionResult> DeleteInstance(int instanceId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _service.DeleteInstanceAsync(instanceId, userId);
            if (!result)
            {
                return NotFound("Instancja nie została znaleziona lub nie należy do tego użytkownika.");
            }
            return NoContent();
        }

    }
}
