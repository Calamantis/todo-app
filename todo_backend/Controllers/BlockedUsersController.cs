using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using todo_backend.Dtos.BlockedUsersDto;
using todo_backend.Services.BlockedUsersService;

namespace todo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlockedUsersController : ControllerBase
    {
        private readonly IBlockedUsersService _blockedUsersService;

        public BlockedUsersController(IBlockedUsersService blockedUsersService)
        {
            _blockedUsersService = blockedUsersService;
        }

        // POST: api/BlockedUser/{targetUserId}
        [Authorize]
        [HttpPost("{targetUserId}")]
        public async Task<IActionResult> BlockUser(int targetUserId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var result = await _blockedUsersService.BlockUserAsync(userId, targetUserId);
            if (!result) return BadRequest("Nie można zablokować tego użytkownika.");

            return Ok();
        }

        // DELETE: api/BlockedUser/{targetUserId}
        [Authorize]
        [HttpDelete("{targetUserId}")]
        public async Task<IActionResult> UnblockUser(int targetUserId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var result = await _blockedUsersService.UnblockUserAsync(userId, targetUserId);
            if (!result) return NotFound();

            return NoContent();
        }

        // GET: api/BlockedUser
        [Authorize]
        [HttpGet("blocked-users-ids")]
        public async Task<ActionResult<IEnumerable<int>>> GetBlockedUsersIds()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var blockedIds = await _blockedUsersService.GetBlockedUserIdsAsync(userId);
            return Ok(blockedIds);
        }

        [Authorize]
        [HttpGet("blocked-users")]
        public async Task<ActionResult<IEnumerable<BlockedUsersDto>>> GetBlockedUsers()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var blockedUsers = await _blockedUsersService.GetBlockedUsersAsync(userId);
            return Ok(blockedUsers);
        }

    }
}
