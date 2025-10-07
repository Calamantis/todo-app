using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using todo_backend.Dtos.ActivityStorage;
using todo_backend.Services.ActivityStorage;

namespace todo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityStorageController : ControllerBase
    {
        private readonly IActivityStorageService _service;

        public ActivityStorageController(IActivityStorageService service)
        {
            _service = service;
        }

        // GET: api/ActivityStorage
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityStorageDto>>> GetUserTemplates()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var templates = await _service.GetUserTemplatesAsync(userId);
            return Ok(templates);
        }

        // GET: api/ActivityStorage/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityStorageDto>> GetTemplate(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var template = await _service.GetTemplateByIdAsync(id, userId);
            if (template == null) return NotFound();

            return Ok(template);
        }

        // POST: api/ActivityStorage
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ActivityStorageDto>> CreateTemplate([FromBody] ActivityStorageDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var result = await _service.CreateTemplateAsync(dto, userId);
            return Ok(result);
        }

        // PUT: api/ActivityStorage/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ActivityStorageDto>> UpdateTemplate(int id, [FromBody] ActivityStorageDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var result = await _service.UpdateTemplateAsync(id, dto, userId);
            if (result == null) return NotFound();

            return Ok(result);
        }

        // DELETE: api/ActivityStorage/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var success = await _service.DeleteTemplateAsync(id, userId);
            if (!success) return NotFound();

            return NoContent();
        }

    }
}
