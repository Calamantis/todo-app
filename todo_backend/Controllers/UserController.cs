using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_backend.Data;
using todo_backend.Models;
using todo_backend.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using todo_backend.Dtos.User;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) 
        {
            _userService = userService;
        }

        // GET: api/users
        //[Authorize]
        [HttpGet("all-users")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            var user = await _userService.GetUserAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // POST: api/users
        //[HttpPost]
        //public async Task<ActionResult<UserResponseDto>> CreateUser(UserCreateDto dto)
        //{
        //    try
        //    {
        //        var user = await _userService.CreateUserAsync(dto);
        //        //return CreatedAtAction(nameof(GetUser), new { id = user.UserId , user); // <-- jezeli chce zwrocic od razu uzytkownika to nalezy w UserResponseDto dodać pole id (lepiej zrobic nowe dto...)
        //        return Ok(user);
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateFullNameDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.UpdateUserAsync(id, dto);
            if (user == null) return NotFound();
            return NoContent(); // tylko potwierdzenie
            // return Ok(user) // od razu zwraca apdejtowango uzytkownika
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
