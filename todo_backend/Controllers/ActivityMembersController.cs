using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;
using todo_backend.Dtos.ActivityMembers;
using todo_backend.Services.ActivityMembersService;

namespace todo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityMembersController : ControllerBase
    {
        private readonly IActivityMembersService _activityMembersService;

        public ActivityMembersController(IActivityMembersService activityMembersService)
        {
            _activityMembersService = activityMembersService;
        }

        //GET przeglądaj wysłane zaproszenia
        [Authorize]
        [HttpGet("browse-invites")]
        public async Task<ActionResult<IEnumerable<FullActivityMembersDto>>> GetSendInvites()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var invites = await _activityMembersService.GetSentInvitesAsync(userId);
            return Ok(invites);
        }

        //GET przeglądaj zaakceptowane zaproszenia
        [Authorize]
        [HttpGet("browse-accepted")]
        public async Task<ActionResult<IEnumerable<FullActivityMembersDto>>> GetAcceptedInvites()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var invites = await _activityMembersService.GetAcceptedInvitesAsync(userId);
            return Ok(invites);
        }

        //POST wyslanie zaproszenia
        [Authorize]
        [HttpPost("send-invitation")]
        public async Task<ActionResult> SendInvite(int activityId, int invitedUserId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var invite = await _activityMembersService.SendInviteAsync(activityId, userId, invitedUserId);

            if (!invite) return BadRequest();

            return NoContent();
        }

        //PATCH akceptacja zaproszenia
        [Authorize]
        [HttpPatch("accept-invitation")]
        public async Task<ActionResult> AcceptInvite(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var accept = await _activityMembersService.AcceptInviteAsync(activityId, userId);
            if (!accept) return BadRequest();
            return NoContent();

        }

        //DELETE usuniecie (cofniecie zaproszenia)
        [Authorize]
        [HttpDelete("revoke-invitation")]
        public async Task<ActionResult> RevokeInvite(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var success = await _activityMembersService.RevokeInviteAsync(activityId, userId);
            if (!success) return BadRequest();
            return NoContent();
        }
    }
}
