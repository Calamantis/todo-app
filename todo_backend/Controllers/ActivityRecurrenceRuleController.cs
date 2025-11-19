using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using todo_backend.Dtos.ActivityRecurrenceRuleDto;
using todo_backend.Services.ActivityRecurrenceRuleService;

namespace todo_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityRecurrenceRuleController : ControllerBase
    {
        private readonly IActivityRecurrenceRuleService _service;

        public ActivityRecurrenceRuleController(IActivityRecurrenceRuleService service)
        {
            _service = service;
        }

        // GET: api/activityrecurrencerule/user/{userId}
        [HttpGet("user/get-recurrence-rules")]
        public async Task<IActionResult> GetRecurrenceRulesByUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var rules = await _service.GetRecurrenceRulesByUserIdAsync(userId);
            return Ok(rules);
        }

        // GET: api/activityrecurrencerule/activity/{activityId}
        [HttpGet("activity/get-activity-recurrence-rules")]
        public async Task<IActionResult> GetRecurrenceRulesByActivityId(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var rules = await _service.GetRecurrenceRulesByActivityIdAsync(activityId, userId);
            return Ok(rules);
        }

        // POST: api/activityrecurrencerule
        [HttpPost]
        public async Task<IActionResult> CreateRecurrenceRule([FromBody] ActivityRecurrenceRuleDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var rule = await _service.CreateRecurrenceRuleAsync(dto, userId);
            if (rule == null) return BadRequest("Nie udało się utworzyć reguły.");
            return CreatedAtAction(nameof(GetRecurrenceRulesByActivityId), new { activityId = rule.ActivityId }, rule);
        }

        // PUT: api/activityrecurrencerule/{recurrenceRuleId}
        [HttpPut("edit-recurrence-rule")]
        public async Task<IActionResult> UpdateRecurrenceRule(int recurrenceRuleId, [FromBody] ActivityRecurrenceRuleDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var rule = await _service.UpdateRecurrenceRuleAsync(userId,recurrenceRuleId, dto);
            if (rule == null) return NotFound("Reguła nie została znaleziona.");
            return Ok(rule);
        }

        // DELETE: api/activityrecurrencerule/{recurrenceRuleId}
        [HttpDelete("delete-recurrence-rule")]
        public async Task<IActionResult> DeleteRecurrenceRule(int recurrenceRuleId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _service.DeleteRecurrenceRuleAsync(recurrenceRuleId, userId);
            if (!result) return NotFound("Reguła nie została znaleziona.");
            return NoContent();
        }
    }
}
