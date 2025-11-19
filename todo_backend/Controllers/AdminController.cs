using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using todo_backend.Services.AdminService;
using todo_backend.Dtos.AdminDto;

namespace todo_backend.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // POST: api/admin/moderators (tworzenie nowego konta moderatora)
        [HttpPost("moderators")]
        public async Task<IActionResult> CreateModerator(AdminCreateModeratorRequestDto request)
        {
            var id = await _adminService.CreateModeratorAccountAsync(
                request.Email,
                request.FullName,
                request.Password
            );

            return NoContent();
        }

        // POST: api/admin/users/{id}/promote-moderator (promocja już istniejącego konta)
        [HttpPost("users/{userId:int}/promote-moderator")]
        public async Task<IActionResult> PromoteToModerator(int userId)
        {
            await _adminService.PromoteToModeratorAsync(userId);
            return NoContent();
        }

        // DELETE: api/admin/users/{id} (usunięcie konta)
        [HttpDelete("users/{userId:int}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            await _adminService.DeleteUserAsync(userId);
            return NoContent();
        }

    }
}
