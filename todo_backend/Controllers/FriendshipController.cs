//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using todo_backend.Data;
//using todo_backend.Models;
//using System.Security.Claims;
//using todo_backend.Services.FriendshipService;
//using todo_backend.Dtos.Friendship;

//namespace todo_backend.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class FriendshipController : ControllerBase
//    {
//        private readonly IFriendshipService _friendshipService;

//        public FriendshipController(IFriendshipService friendshipService)
//        {
//            _friendshipService = friendshipService;
//        }

//        [HttpPatch("accept/{userId}/{friendId}")]
//        public async Task<IActionResult> AcceptFriendship(int userId, int friendId)
//        {
//            var success = await _friendshipService.AcceptFriendshipAsync(userId, friendId);
//            if (!success) return NotFound();
//            return NoContent();
//        }

//        [Authorize]
//        [HttpGet("me")]
//        public async Task<ActionResult<IEnumerable<FullFriendshipDetailDto>>> GetMyFriends()
//        {
//            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
//            if (userIdClaim == null)
//                return Unauthorized("Brak claimu 'sub' w tokenie.");

//            var userId = int.Parse(userIdClaim.Value);

//            var friends = await _friendshipService.GetUserFriendshipsAsync(userId);
//            return Ok(friends);
//        }

//        [Authorize]
//        [HttpGet("debug/claims")]
//        public IActionResult DebugClaims()
//        {
//            return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
//        }

//        // GET: api/friendship/{userId}/sent
//        [HttpGet("{userId}/sent")]
//        public async Task<ActionResult<IEnumerable<FullFriendshipDetailDto>>> GetSentInvites(int userId)
//        {
//            var invites = await _friendshipService.GetSentInvitesAsync(userId);
//            return Ok(invites);
//        }

//        // GET: api/friendship/{userId}/received
//        [HttpGet("{userId}/received")]
//        public async Task<ActionResult<IEnumerable<FullFriendshipDetailDto>>> GetReceivedInvites(int userId)
//        {
//            var invites = await _friendshipService.GetReceivedInvitesAsync(userId);
//            return Ok(invites);
//        }

//        // DELETE: api/friendship/remove/(userId}/{friendId}
//        [HttpDelete("remove/{userId}/{friendId}")]
//        public async Task<IActionResult> RemoveFriend(int userId, int friendId)
//        {
//            var success = await _friendshipService.RemoveFriendAsync(userId, friendId);
//            if (!success) return NotFound("Friendship not found.");
//            return NoContent();
//        }
//    }




//}
