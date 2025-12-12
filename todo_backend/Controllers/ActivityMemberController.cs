using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;
using todo_backend.Dtos.ActivityMemberDto;
using todo_backend.Models;
using todo_backend.Services.ActivityMembersService;

namespace todo_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityMemberController : ControllerBase
    {
        private readonly IActivityMemberService _activityMemberService;

        public ActivityMemberController(IActivityMemberService activityMemberService)
        {
            _activityMemberService = activityMemberService;
        }

        //GET przeglądaj aktywności online
        [HttpGet("browse-online-activities")]
        public async Task<IActionResult> GetOnlineActivities()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _activityMemberService.GetOnlineActivitiesAsync(userId);
            return Ok(result);
        }

        [HttpGet("browse-recieved-invites")]
        public async Task<IActionResult> GetReceivedInvites()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _activityMemberService.GetReceivedInvitesAsync(userId);
            return Ok(result);
        }

        [HttpGet("browse-participants")]
        public async Task<IActionResult> GetAcceptedMembers(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _activityMemberService.GetAcceptedMembersAsync(activityId, userId);
            return Ok(result);
        }

        [HttpGet("browse-sent-invites")]
        public async Task<IActionResult> GetSentInvites(int activityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _activityMemberService.GetSentInvitesAsync(activityId, userId);
            return Ok(result);
        }

        [HttpPost("send-invite")]
        public async Task<IActionResult> SendInvite(int activityId,int invitedUserId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _activityMemberService.SendInviteAsync(userId, activityId, invitedUserId);
            return Ok(result);
        }

        [HttpPost("join-by-code")]
        public async Task<IActionResult> JoinByCode(string joinCode)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _activityMemberService.JoinByCodeAsync(userId, joinCode);
            return Ok(result);
        }

        [HttpPatch("update-invite-status")]
        public async Task<IActionResult> UpdateStatus(int activityId, ActivityMemberStatusUpdateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _activityMemberService.UpdateInviteStatusAsync(userId, activityId, dto.Status);
            return Ok(result);
        }

        [HttpDelete("cancel-invite")]
        public async Task<IActionResult> CancelInvite(int activityId, int targetUserId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int ownerId = int.Parse(userIdClaim.Value);

            var result = await _activityMemberService.CancelInviteAsync(ownerId, activityId, targetUserId);
            return Ok(result);
        }

        [HttpDelete("remove-participant")]
        public async Task<IActionResult> RemoveParticipant(int activityId, int targetUserId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int ownerId = int.Parse(userIdClaim.Value);


            var result = await _activityMemberService.RemoveMemberAsync(ownerId, activityId, targetUserId);
            return Ok(result);
        }

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

    }
}
