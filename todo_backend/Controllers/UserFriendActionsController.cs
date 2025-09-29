using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using todo_backend.Data;
using todo_backend.Dtos.Friendship;
using todo_backend.Dtos.User;
using todo_backend.Models;
using todo_backend.Services.FriendshipService;
using todo_backend.Services.UserFriendActions;

namespace todo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserFriendActionsController : ControllerBase
    {
        private readonly IUserFriendActionsService _userFriendActionsService;

        public UserFriendActionsController(IUserFriendActionsService userFriendActionService)
        {
            _userFriendActionsService = userFriendActionService;
        }




        //GET user's friendships
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<IEnumerable<FriendshipDto>>> GetMyFriends()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var friends = await _userFriendActionsService.GetUserFriendshipsAsync(userId);
            return Ok(friends);
        }

        //GET user's sent friend requests
        [Authorize]
        [HttpGet("sent-invites")]
        public async Task<ActionResult<IEnumerable<FriendshipDto>>> GetSentInvites()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var invites = await _userFriendActionsService.GetSentInvitesAsync(userId);
            return Ok(invites);
        }

        //GET user's recieved friend requests
        [Authorize]
        [HttpGet("recieved-invites")]
        public async Task<ActionResult<IEnumerable<FriendshipDto>>> GetReceivedInvites()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var invites = await _userFriendActionsService.GetReceivedInvitesAsync(userId);
            return Ok(invites);
        }


        //GET users -> browse users (to send friend request to)
        [Authorize]
        [HttpGet("browse-users")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> BrowseUsers()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var users = await _userFriendActionsService.BrowseUsersAsync(userId);
            return Ok(users);
        }

        //POST send invite
        [HttpPost("sent-invite")]
        public async Task<IActionResult> AddFriendship(FriendshipCreateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var friendship = await _userFriendActionsService.AddFriendshipAsync(userId, dto.FriendId);
            if (friendship == null) return BadRequest("Friendship already exists or invalid.");
            return NoContent();
        }

        //PATCH accept recieved friend request
        [Authorize]
        [HttpPatch("accept-invite")]
        public async Task<IActionResult> AcceptFriendship(int friendId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var success = await _userFriendActionsService.AcceptFriendshipAsync(userId, friendId);
            if (!success) return NotFound();
            return NoContent();
        }

        //DELETE cancel send invite
        [Authorize]
        [HttpDelete("cancel-send-invite")]
        public async Task<IActionResult> CancelInvite(int friendId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _userFriendActionsService.CancelFriendshipInviteAsync(userId, friendId);

            if (!result)
                return NotFound("No pending invite to this user.");

            return Ok("Friend request canceled.");
        }

        //DELETE decline friend invite
        [Authorize]
        [HttpDelete("reject-invite")]
        public async Task<IActionResult> RejectInvite(int requesterId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var result = await _userFriendActionsService.DeleteFriendshipInviteAsync(userId, requesterId);

            if (!result)
                return NotFound("No pending invite from this user.");

            return Ok("Friend request deleted.");
        }

        //DELETE delete friend from friendlist
        [Authorize]
        [HttpDelete("remove-friend")]
        public async Task<IActionResult> RemoveFriend(int friendId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var success = await _userFriendActionsService.RemoveFriendAsync(userId, friendId);
            if (!success) return NotFound("Friendship not found.");
            return NoContent();
        }

        //limity na pola (w encji i dto tworzących)
        //ismodelvalid --> tutaj chyba w sumie nie ma zbyt co namieszac wiec nie wiem czy needed
    }
}
