using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using todo_backend.Data;
using todo_backend.Dtos.User;
using todo_backend.Models;
using todo_backend.Services.UserAccountService;

namespace todo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAccountController : ControllerBase
    {
        private readonly IUserAccountService _userAccountService;

        public UserAccountController(IUserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        [Authorize]
        [HttpGet("account-details")]
        public async Task<ActionResult<UserResponseDto>> GetMe()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var user = await _userAccountService.GetUserDetailsAsync(userId);
            if (user == null) return NotFound();

            return Ok(user);
        }

        [Authorize]
        [HttpPut("change-name")]
        public async Task<IActionResult> UpdateUser(UpdateFullNameDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userAccountService.UpdateUserAsync(userId, dto);
            if (user == null) return NotFound();

            return NoContent();
        }

        [Authorize]
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var success = await _userAccountService.DeleteUserAsync(userId);
            if (!success) return NotFound();

            return NoContent();
        }

        [Authorize]
        [HttpPost("allow-mentions")]
        public async Task<IActionResult> UpdateAllowMentions(UpdateAllowMentionsDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var success = await _userAccountService.ToggleAllowMentionsAsync(userId);
            if (success == null) return NotFound();

            return NoContent();
        }

        [Authorize]
        [HttpPost("allow-friend-invites")]
        public async Task<IActionResult> UpdateAllowFriendInvites(UpdateAllowFriendInvitesDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var success = await _userAccountService.ToggleAllowFriendInvitesAsync(userId);
            if (success == null) return NotFound();

            return NoContent();
        }

    }
}
