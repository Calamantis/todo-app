using Microsoft.AspNetCore.Mvc;
using todo_backend.Dtos.Auth;
using todo_backend.Dtos.User;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authService.CreateUserAsync(dto);
            if (user == null) return BadRequest("Email already exists!");
            return Ok(user);
        }
    }
}
