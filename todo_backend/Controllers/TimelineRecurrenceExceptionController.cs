using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        // GET /api/recurrence-exceptions/activity/5
        [Authorize]
        [HttpGet("activity/{activityId}")]
        public async Task<ActionResult<IEnumerable<TimelineRecurrenceExceptionResponseDto>>> GetAll(int activityId)
        {
            var result = await _recurrenceExceptionService.GetAllAsync(activityId);
            return Ok(result);
        }

        // GET /api/recurrence-exceptions/10
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<TimelineRecurrenceExceptionResponseDto>> GetById(int id)
        {
            var result = await _recurrenceExceptionService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST /api/recurrence-exceptions
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<TimelineRecurrenceExceptionResponseDto>> Create([FromBody] TimelineRecurrenceExceptionCreateDto dto)
        {
            var result = await _recurrenceExceptionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.ExceptionId }, result);
        }

        // PUT /api/recurrence-exceptions/10
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<TimelineRecurrenceExceptionResponseDto>> Update(int id, [FromBody] TimelineRecurrenceExceptionUpdateDto dto)
        {
            var result = await _recurrenceExceptionService.UpdateAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // DELETE /api/recurrence-exceptions/10
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _recurrenceExceptionService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

    }
}
