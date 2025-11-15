using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;
using todo_backend.Dtos.ActivityMemberDto;
using todo_backend.Models;
using todo_backend.Services.ActivityMembersService;

namespace todo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityMemberController : ControllerBase
    {
        private readonly IActivityMemberService _activityMemberService;

        public ActivityMemberController(IActivityMemberService activityMemberService)
        {
            _activityMemberService = activityMemberService;
        }

        ////GET przeglądaj dostane zaproszenia
        //[Authorize]
        //[HttpGet("browse-recieved")]
        //public async Task<ActionResult> GetRevievedInvites()
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //    if (userIdClaim == null) return Unauthorized();
        //    int userId = int.Parse(userIdClaim.Value);

        //    var invites = await _activityMembersService.GetSentInvitesAsync(userId);
        //    if (invites == null) return NotFound();
        //    return Ok(invites);
        //}

        [HttpGet("browse-recieved-invites")]
        public async Task<IActionResult> GetReceivedInvites()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _activityMemberService.GetReceivedInvitesAsync(userId);
            return Ok(result);
        }

        ////GET przeglądaj zaakceptowane zaproszenia
        //[Authorize]
        //[HttpGet("show-accepted")]
        //public async Task<ActionResult> GetAcceptedInvites()
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //    if (userIdClaim == null) return Unauthorized();
        //    int userId = int.Parse(userIdClaim.Value);

        //    var invites = await _activityMembersService.GetAcceptedInvitesAsync(userId);
        //    if (invites == null) return NotFound();
        //    return Ok(invites);
        //}

        [HttpGet("browse-participants")]
        public async Task<IActionResult> GetAcceptedMembers(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _activityMemberService.GetAcceptedMembersAsync(activityId);
            return Ok(result);
        }

        ////GET przeglądaj wysłane zaproszenia
        //[Authorize]
        //[HttpGet("browse-invites")]
        //public async Task<ActionResult<IEnumerable<FullActivityMembersDto>>> GetSendInvites(int activityId)
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //    if (userIdClaim == null) return Unauthorized();
        //    int userId = int.Parse(userIdClaim.Value);

        //    var invites = await _activityMembersService.GetSentInvitesAsync(activityId, userId);
        //    return Ok(invites);
        //}

        [HttpGet("browse-sent-invites")]
        public async Task<IActionResult> GetSentInvites(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _activityMemberService.GetSentInvitesAsync(activityId, userId);
            return Ok(result);
        }

        ////POST wyslanie zaproszenia
        //[Authorize]
        //[HttpPost("send-invitation")]
        //public async Task<ActionResult> SendInvite(int activityId, int invitedUserId)
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //    if (userIdClaim == null) return Unauthorized();
        //    int userId = int.Parse(userIdClaim.Value);

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var invite = await _activityMembersService.SendInviteAsync(activityId, userId, invitedUserId);

        //    if (invite == null)
        //        return BadRequest("Error sending request.");

        //    return NoContent();
        //}

        [HttpPost("send-invite")]
        public async Task<IActionResult> SendInvite(int activityId,int invitedUserId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _activityMemberService.SendInviteAsync(userId, activityId, invitedUserId);
            return Ok(result);
        }

        ////POST dolaczenie po kodzie aktywnosci (dla osob spoza listy znajomych)
        //[Authorize]
        //[HttpPost("join-by-code/{joinCode}")]
        //public async Task<IActionResult> JoinByCode(string joinCode)
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //    if (userIdClaim == null) return Unauthorized();

        //    int userId = int.Parse(userIdClaim.Value);

        //    var success = await _activityMembersService.JoinActivityByCodeAsync(joinCode, userId);
        //    if (!success) return BadRequest("Invalid or expired code.");

        //    return Ok("Successfully joined the activity.");
        //}

        [HttpPost("join-by-code")]
        public async Task<IActionResult> JoinByCode(string joinCode)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _activityMemberService.JoinByCodeAsync(userId, joinCode);
            return Ok(result);
        }

        //[Authorize]
        //[HttpPatch("update-invite-status")]
        //public async Task<IActionResult> UpdateInviteStatus(int activityId, [FromBody] UpdateInviteStatusDto dto)
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //    if (userIdClaim == null) return Unauthorized();
        //    int userId = int.Parse(userIdClaim.Value);

        //    var success = await _activityMembersService.UpdateInviteStatusAsync(activityId, userId, dto.Status);

        //    if (!success)
        //        return BadRequest("Nie można zmienić statusu zaproszenia (brak zaproszenia lub niepoprawny status).");

        //    return NoContent();
        //}

        [HttpPatch("update-invite-status")]
        public async Task<IActionResult> UpdateStatus(int activityId, ActivityMemberStatusUpdateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _activityMemberService.UpdateInviteStatusAsync(userId, activityId, dto.Status);
            return Ok(result);
        }

        //// DELETE: api/activitymember/revoke-invite/5?targetUserId=10
        //[Authorize]
        //[HttpDelete("revoke-invite")]
        //public async Task<IActionResult> RevokeInvite(int activityId, [FromQuery] int targetUserId)
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //    if (userIdClaim == null) return Unauthorized();
        //    int ownerId = int.Parse(userIdClaim.Value);

        //    var success = await _activityMembersService.RevokeInviteAsync(activityId, targetUserId, ownerId);
        //    if (!success) return BadRequest("Nie można cofnąć zaproszenia (brak zaproszenia pending lub nie jesteś ownerem).");

        //    return NoContent();
        //}

        [HttpDelete("cancel-invite")]
        public async Task<IActionResult> CancelInvite(int activityId, int targetUserId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int ownerId = int.Parse(userIdClaim.Value);

            var result = await _activityMemberService.CancelInviteAsync(ownerId, activityId, targetUserId);
            return Ok(result);
        }

        //// DELETE: api/activitymember/remove-participant/5?targetUserId=10
        //[Authorize]
        //[HttpDelete("remove-participant")]
        //public async Task<IActionResult> RemoveParticipant(int activityId, [FromQuery] int targetUserId)
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //    if (userIdClaim == null) return Unauthorized();
        //    int ownerId = int.Parse(userIdClaim.Value);

        //    var success = await _activityMembersService.RemoveParticipantAsync(activityId, targetUserId, ownerId);
        //    if (!success) return BadRequest("Nie można usunąć uczestnika (brak użytkownika lub nie jesteś ownerem).");

        //    return NoContent();
        //}

        [HttpDelete("remove-participant")]
        public async Task<IActionResult> RemoveParticipant(int activityId, int targetUserId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int ownerId = int.Parse(userIdClaim.Value);


            var result = await _activityMemberService.RemoveMemberAsync(ownerId, activityId, targetUserId);
            return Ok(result);
        }

        ////DELETE usuniecie swojej obecnosci z aktywnosci
        //[Authorize]
        //[HttpDelete("cancel-attendance")]
        //public async Task<ActionResult> CancelInvite(int activityId)
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //    if (userIdClaim == null) return Unauthorized();
        //    int userId = int.Parse(userIdClaim.Value);

        //    var success = await _activityMembersService.CancelInviteAsync(activityId, userId);
        //    if (!success) return BadRequest();
        //    return NoContent();
        //}

        [HttpDelete("leave-activity")]
        public async Task<IActionResult> LeaveActivity(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _activityMemberService.LeaveActivityAsync(userId, activityId);
            return Ok(result);
        }

        [HttpDelete("remove-and-block")]
        public async Task<IActionResult> RemoveAndBlock(int activityId, int targetUserId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _activityMemberService.RemoveAndBlockAsync(userId, activityId, targetUserId);
            return Ok(result);
        }

        ////DELETE usuwa wszystkich uczestnikow z aktywnosci
        //[Authorize]
        //[HttpDelete("remove-everyone")]
        //public async Task<IActionResult> DeleteActivity(int activityId)
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //    if (userIdClaim == null) return Unauthorized();
        //    int userId = int.Parse(userIdClaim.Value);

        //    var success = await _activityMembersService.CancelActivityAsync(activityId, userId);

        //    if (!success)
        //        return NotFound("Activity not found or you are not the owner.");

        //    return NoContent();
        //}

    }
}
