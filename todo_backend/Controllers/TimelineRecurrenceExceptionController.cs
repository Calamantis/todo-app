using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using todo_backend.Dtos.TimelineRecurrenceExceptionDto;
using todo_backend.Services.TimelineRecurrenceExceptionService;

namespace todo_backend.Controllers
{

    [ApiController]
    [Route("api/recurrence-exceptions")]
    public class TimelineRecurrenceExceptionController : ControllerBase
    {
        private readonly ITimelineRecurrenceExceptionService _recurrenceExceptionService;

        public TimelineRecurrenceExceptionController(ITimelineRecurrenceExceptionService service)
        {
            _recurrenceExceptionService = service;
        }

 
        [Authorize]
        [HttpGet("{activityId}")]
        public async Task<ActionResult<IEnumerable<TimelineRecurrenceExceptionResponseDto>>> GetForActivity(int activityId)
        {
            var result = await _recurrenceExceptionService.GetExceptionsForActivityAsync(activityId);
            return Ok(result);
        }

        //// GET /api/recurrence-exceptions/10
        //[Authorize]
        //[HttpGet("{id}")]
        //public async Task<ActionResult<TimelineRecurrenceExceptionResponseDto>> GetById(int id)
        //{
        //    var result = await _recurrenceExceptionService.GetByIdAsync(id);
        //    if (result == null) return NotFound();
        //    return Ok(result);
        //}

        // POST /api/recurrence-exceptions
        [Authorize]
        [HttpPost("recurrence-exceptions")]
        public async Task<ActionResult<TimelineRecurrenceExceptionResponseDto>> Create([FromBody] TimelineRecurrenceExceptionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _recurrenceExceptionService.CreateExceptionAsync(dto);
            return CreatedAtAction(nameof(GetForActivity), new { activityId = dto.ActivityId }, result);
        }

        //// PUT /api/recurrence-exceptions/10
        //[Authorize]
        //[HttpPut("{id}")]
        //public async Task<ActionResult<TimelineRecurrenceExceptionResponseDto>> Update(int id, [FromBody] TimelineRecurrenceExceptionUpdateDto dto)
        //{
        //    var result = await _recurrenceExceptionService.UpdateAsync(id, dto);
        //    if (result == null) return NotFound();
        //    return Ok(result);
        //}

        // DELETE /api/recurrence-exceptions/10
        [Authorize]
        [HttpDelete("{exceptionId}")]
        public async Task<IActionResult> Delete(int exceptionId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _recurrenceExceptionService.DeleteExceptionAsync(exceptionId, userId);
            if (!success)
                return NotFound("Exception not found or not owned by current user.");

            return NoContent();
        }

    }
}
