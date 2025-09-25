using Microsoft.AspNetCore.Mvc;
using todo_backend.Dtos;
using todo_backend.Services.AuthService;

namespace todo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.AuthenticateAsync(dto);
            if (result == null)
                return Unauthorized("Invalid email or password");

            return Ok(result);
        }
    }
}
