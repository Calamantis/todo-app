using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using todo_backend.Services.ModerationService;
using todo_backend.Dtos.ModerationDto;

namespace todo_backend.Controllers
{

    [Authorize(Roles = "Admin,Moderator")]
    [ApiController]
    [Route("api/moderation")]
    public class ModerationController : ControllerBase
    {
        private readonly IModerationService _moderationService;

        public ModerationController(IModerationService moderationService)
        {
            _moderationService = moderationService;
        }


        // ======= USERS =======

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _moderationService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPut("users/{userId:int}/profile-image")]
        public async Task<IActionResult> UpdateProfileImage(int userId, string newValue)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int moderatorId = int.Parse(userIdClaim.Value);

            await _moderationService.UpdateUserProfileImageAsync(moderatorId, userId, newValue);
            return NoContent();
        }

        [HttpDelete("users/{userId:int}/profile-image")]
        public async Task<IActionResult> ResetProfileImage(int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int moderatorId = int.Parse(userIdClaim.Value);


            await _moderationService.ResetUserProfileImageAsync(moderatorId, userId);
            return NoContent();
        }

        [HttpPut("users/{userId:int}/background-image")]
        public async Task<IActionResult> UpdateBackgroundImage(int userId, string newValue)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int moderatorId = int.Parse(userIdClaim.Value);


            await _moderationService.UpdateUserBackgroundImageAsync(moderatorId, userId, newValue);
            return NoContent();
        }

        [HttpDelete("users/{userId:int}/background-image")]
        public async Task<IActionResult> ResetBackgroundImage(int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int moderatorId = int.Parse(userIdClaim.Value);


            await _moderationService.ResetUserBackgroundImageAsync(moderatorId,userId);
            return NoContent();
        }

        [HttpPut("users/{userId:int}/display-name")]
        public async Task<IActionResult> UpdateDisplayName(int userId, string newValue)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int moderatorId = int.Parse(userIdClaim.Value);


            await _moderationService.UpdateUserDisplayNameAsync(moderatorId, userId, newValue);
            return NoContent();
        }

        [HttpDelete("users/{userId:int}/display-name")]
        public async Task<IActionResult> ResetDisplayName(int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int moderatorId = int.Parse(userIdClaim.Value);


            await _moderationService.ResetUserDisplayNameAsync(moderatorId, userId);
            return NoContent();
        }

        [HttpPut("users/{userId:int}/description")]
        public async Task<IActionResult> UpdateDescription(int userId, string newValue)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int moderatorId = int.Parse(userIdClaim.Value);


            await _moderationService.UpdateUserDescriptionAsync(moderatorId, userId, newValue);
            return NoContent();
        }

        [HttpDelete("users/{userId:int}/description")]
        public async Task<IActionResult> ResetDescription(int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int moderatorId = int.Parse(userIdClaim.Value);


            await _moderationService.ResetUserDescriptionAsync(moderatorId, userId);
            return NoContent();
        }

        // ======= ACTIVITIES =======

        [HttpGet("activities")]
        public async Task<IActionResult> GetAllActivities()
        {
            var list = await _moderationService.GetAllActivitiesAsync();
            return Ok(list);
        }

        [HttpPut("activities/{activityId:int}/title")]
        public async Task<IActionResult> UpdateActivityTitle(int activityId, string newValue)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int moderatorId = int.Parse(userIdClaim.Value);


            await _moderationService.UpdateActivityTitleAsync(moderatorId, activityId, newValue);
            return NoContent();
        }

        [HttpDelete("activities/{activityId:int}/title")]
        public async Task<IActionResult> ResetActivityTitle(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int moderatorId = int.Parse(userIdClaim.Value);


            await _moderationService.ResetActivityTitleAsync(moderatorId, activityId);
            return NoContent();
        }

        [HttpPut("activities/{activityId:int}/description")]
        public async Task<IActionResult> UpdateActivityDescription(int activityId, string newValue)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int moderatorId = int.Parse(userIdClaim.Value);


            await _moderationService.UpdateActivityDescriptionAsync(moderatorId, activityId, newValue);
            return NoContent();
        }

        [HttpDelete("activities/{activityId:int}/description")]
        public async Task<IActionResult> ResetActivityDescription(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int moderatorId = int.Parse(userIdClaim.Value);


            await _moderationService.ResetActivityDescriptionAsync(moderatorId, activityId);
            return NoContent();
        }


    }
}
