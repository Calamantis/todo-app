using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using todo_backend.Dtos.InstanceExclusionDto;
using todo_backend.Services.InstanceExclusionService;

namespace todo_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/exclusions")]
    public class InstanceExclusionController : ControllerBase
    {
        private readonly IInstanceExclusionService _instanceExclusionService;

        public InstanceExclusionController(IInstanceExclusionService instanceExclusionService)
        {
            _instanceExclusionService = instanceExclusionService;
        }

        [HttpGet("get-user-exclusions")]
        public async Task<IActionResult> GetByUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _instanceExclusionService.GetByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpGet("get-activity-exclusion")]
        public async Task<IActionResult> GetByActivity(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _instanceExclusionService.GetByActivityAndUserAsync(userId, activityId);
            return Ok(result);
        }
        [HttpPost("create-exclusion")]
        public async Task<IActionResult> Create(InstanceExclusionCreateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _instanceExclusionService.CreateAsync(userId, dto);
            return Ok(result);
        }

        [HttpPut("edit-exclusion")]
        public async Task<IActionResult> Update(int exclusionId, InstanceExclusionUpdateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _instanceExclusionService.UpdateAsync(exclusionId, dto, userId);
            return Ok(result);
        }

        [HttpDelete("delete-exclusion")]
        public async Task<IActionResult> Delete(int exclusionId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _instanceExclusionService.DeleteAsync(exclusionId, userId);
            return result ? Ok() : NotFound();
        }

    }
}
