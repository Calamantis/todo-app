using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_backend.Data;
using todo_backend.Dtos;
using todo_backend.Models;
using todo_backend.Services.FriendshipService;

namespace todo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendshipController : ControllerBase
    {
        private readonly IFriendshipService _friendshipService;

        public FriendshipController(IFriendshipService friendshipService)
        {
            _friendshipService = friendshipService;
        }

        [HttpPost]
        public async Task<IActionResult> AddFriendship(FriendshipCreateDto dto)
        {
            var friendship = await _friendshipService.AddFriendshipAsync(dto.UserId, dto.FriendId);
            if (friendship == null) return BadRequest("Friendship already exists or invalid.");
            return Ok(friendship);
        }

        [HttpPatch("accept/{userId}/{friendId}")]
        public async Task<IActionResult> AcceptFriendship(int userId, int friendId)
        {
            var success = await _friendshipService.AcceptFriendshipAsync(userId, friendId);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("{userId}/friends")]
        public async Task<ActionResult<IEnumerable<FriendshipResponseDto>>> GetFriendships(int userId)
        {
            var friendships = await _friendshipService.GetUserFriendshipsAsync(userId);
            return Ok(friendships);
        }

        // GET: api/friendship/{userId}/sent
        [HttpGet("{userId}/sent")]
        public async Task<ActionResult<IEnumerable<FriendshipResponseDto>>> GetSentInvites(int userId)
        {
            var invites = await _friendshipService.GetSentInvitesAsync(userId);
            return Ok(invites);
        }

        // GET: api/friendship/{userId}/received
        [HttpGet("{userId}/received")]
        public async Task<ActionResult<IEnumerable<FriendshipResponseDto>>> GetReceivedInvites(int userId)
        {
            var invites = await _friendshipService.GetReceivedInvitesAsync(userId);
            return Ok(invites);
        }

        // DELETE: api/friendship/remove/(userId}/{friendId}
        [HttpDelete("remove/{userId}/{friendId}")]
        public async Task<IActionResult> RemoveFriend(int userId, int friendId)
        {
            var success = await _friendshipService.RemoveFriendAsync(userId, friendId);
            if (!success) return NotFound("Friendship not found.");
            return NoContent();
        }
    }




}
