//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using todo_backend.Dtos.TimelineRecurrenceInstanceDto;
//using todo_backend.Services.TimelineRecurrenceInstanceService;

//namespace todo_backend.Controllers
//{

//    [ApiController]
//    [Route("api/recurrence-instances")]
//    public class TimelineRecurrenceInstanceController : ControllerBase
//    {
//        private readonly ITimelineRecurrenceInstanceService _recurrenceInstanceService;

//        public TimelineRecurrenceInstanceController(ITimelineRecurrenceInstanceService service)
//        {
//            _recurrenceInstanceService = service;
//        }

//        // GET /api/recurrence-instances/activity/5
//        [Authorize]
//        [HttpGet("activity/{activityId}")]
//        public async Task<ActionResult<IEnumerable<TimelineRecurrenceInstanceResponseDto>>> GetAll(int activityId)
//        {
//            var result = await _recurrenceInstanceService.GetAllAsync(activityId);
//            return Ok(result);
//        }

//        // GET /api/recurrence-instances/10
//        [Authorize]
//        [HttpGet("{id}")]
//        public async Task<ActionResult<TimelineRecurrenceInstanceResponseDto>> GetById(int id)
//        {
//            var result = await _recurrenceInstanceService.GetByIdAsync(id);
//            if (result == null) return NotFound();
//            return Ok(result);
//        }

//        // POST /api/recurrence-instances
//        [Authorize]
//        [HttpPost]
//        public async Task<ActionResult<TimelineRecurrenceInstanceResponseDto>> Create([FromBody] TimelineRecurrenceInstanceCreateDto dto)
//        {
//            var result = await _recurrenceInstanceService.CreateAsync(dto);
//            return CreatedAtAction(nameof(GetById), new { id = result.InstanceId }, result);
//        }

//        // PUT /api/recurrence-instances/10
//        [Authorize]
//        [HttpPut("{id}")]
//        public async Task<ActionResult<TimelineRecurrenceInstanceResponseDto>> Update(int id, [FromBody] TimelineRecurrenceInstanceUpdateDto dto)
//        {
//            var result = await _recurrenceInstanceService.UpdateAsync(id, dto);
//            if (result == null) return NotFound();
//            return Ok(result);
//        }

//        // DELETE /api/recurrence-instances/10
//        [Authorize]
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var success = await _recurrenceInstanceService.DeleteAsync(id);
//            return success ? NoContent() : NotFound();
//        }

//    }
//}
