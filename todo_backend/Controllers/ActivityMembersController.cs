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
        //GET przeglądaj dostane zaproszenia
        [Authorize]
        [HttpGet("browse-recieved")]
        public async Task<ActionResult> GetRevievedInvites()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var invites = await _activityMembersService.GetSentInvitesAsync(userId);
            if (invites == null) return NotFound();
            return Ok(invites);
        }
        //GET przeglądaj zaakceptowane zaproszenia
        [Authorize]
        [HttpGet("show-accepted")]
        public async Task<ActionResult> GetAcceptedInvites()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var invites = await _activityMembersService.GetAcceptedInvitesAsync(userId);
            if (invites == null) return NotFound();
            return Ok(invites);
        }

        //GET przeglądaj wysłane zaproszenia
        [Authorize]
        [HttpGet("browse-invites")]
        public async Task<ActionResult<IEnumerable<FullActivityMembersDto>>> GetSendInvites(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var invites = await _activityMembersService.GetSentInvitesAsync(activityId, userId);
            return Ok(invites);
        }

        //GET zwraca uczestnikow aktywnosci
        [Authorize]
        [HttpGet("show-participants")]
        public async Task<ActionResult<IEnumerable<FullActivityMembersDto>>> ShowParticipants(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var participants = await _activityMembersService.GetParticipantsOfActivityAsync(activityId, userId);
            if (participants == null) return NotFound();
            return Ok(participants);
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

            if (invite == null)
                return BadRequest("Error sending request.");

            return NoContent();
        }

        //POST dolaczenie po kodzie aktywnosci (dla osob spoza listy znajomych)
        [Authorize]
        [HttpPost("join-by-code/{joinCode}")]
        public async Task<IActionResult> JoinByCode(string joinCode)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var success = await _activityMembersService.JoinActivityByCodeAsync(joinCode, userId);
            if (!success) return BadRequest("Invalid or expired code.");

            return Ok("Successfully joined the activity.");
        }


        //PATCH akceptacja zaproszenia
        //[Authorize]
        //[HttpPatch("accept-invitation")]
        //public async Task<ActionResult> AcceptInvite(int activityId)
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //    if (userIdClaim == null) return Unauthorized();
        //    int userId = int.Parse(userIdClaim.Value);

        //    var accept = await _activityMembersService.AcceptInviteAsync(activityId, userId);
        //    if (!accept) return BadRequest();
        //    return NoContent();

        //}

        [Authorize]
        [HttpPatch("update-invite-status")]
        public async Task<IActionResult> UpdateInviteStatus(int activityId, [FromBody] UpdateInviteStatusDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var success = await _activityMembersService.UpdateInviteStatusAsync(activityId, userId, dto.Status);

            if (!success)
                return BadRequest("Nie można zmienić statusu zaproszenia (brak zaproszenia lub niepoprawny status).");

            return NoContent();
        }


        //DELETE usuniecie (cofniecie zaproszenia)
        //[Authorize]
        //[HttpDelete("revoke-invitation")]
        //public async Task<ActionResult> RevokeInvite(int activityId)
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //    if (userIdClaim == null) return Unauthorized();
        //    int userId = int.Parse(userIdClaim.Value);

        //    var success = await _activityMembersService.RevokeInviteAsync(activityId, userId);
        //    if (!success) return BadRequest();
        //    return NoContent();
        //}

        // DELETE: api/activitymember/revoke-invite/5?targetUserId=10
        [Authorize]
        [HttpDelete("revoke-invite")]
        public async Task<IActionResult> RevokeInvite(int activityId, [FromQuery] int targetUserId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int ownerId = int.Parse(userIdClaim.Value);

            var success = await _activityMembersService.RevokeInviteAsync(activityId, targetUserId, ownerId);
            if (!success) return BadRequest("Nie można cofnąć zaproszenia (brak zaproszenia pending lub nie jesteś ownerem).");

            return NoContent();
        }

        // DELETE: api/activitymember/remove-participant/5?targetUserId=10
        [Authorize]
        [HttpDelete("remove-participant")]
        public async Task<IActionResult> RemoveParticipant(int activityId, [FromQuery] int targetUserId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int ownerId = int.Parse(userIdClaim.Value);

            var success = await _activityMembersService.RemoveParticipantAsync(activityId, targetUserId, ownerId);
            if (!success) return BadRequest("Nie można usunąć uczestnika (brak użytkownika lub nie jesteś ownerem).");

            return NoContent();
        }

        //DELETE usuniecie swojej obecnosci z aktywnosci
        [Authorize]
        [HttpDelete("cancel-attendance")]
        public async Task<ActionResult> CancelInvite(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var success = await _activityMembersService.CancelInviteAsync(activityId, userId);
            if (!success) return BadRequest();
            return NoContent();
        }

        //DELETE usuwa wszystkich uczestnikow z aktywnosci
        [Authorize]
        [HttpDelete("remove-everyone")]
        public async Task<IActionResult> DeleteActivity(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var success = await _activityMembersService.CancelActivityAsync(activityId, userId);

            if (!success)
                return NotFound("Activity not found or you are not the owner.");

            return NoContent();
        }

    }
}
