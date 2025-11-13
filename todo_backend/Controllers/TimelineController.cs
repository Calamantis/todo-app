using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using todo_backend.Services.TimelineService;

namespace todo_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TimelineController : ControllerBase
    {
        private readonly ITimelineService _timelineService;
        public TimelineController(ITimelineService timelineService)
        {
            _timelineService = timelineService;
        }

        // GET: api/activitytimeline/user/{userId}?from={startDate}&to={endDate}
        [HttpGet("user/get-timeline")]
        public async Task<IActionResult> GetTimelineForUserAsync(int userId, [FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            // Sprawdzamy, czy użytkownik jest właścicielem danych (Claim z JWT)
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || int.Parse(userIdClaim.Value) != userId)
            {
                return Unauthorized("Użytkownik nie jest autoryzowany do tego żądania.");
            }

            // Generowanie instancji aktywności w zadanym przedziale czasu
            await _timelineService.GenerateActivityInstancesAsync(userId, from, to);

            // Pobieranie instancji aktywności użytkownika w określonym przedziale czasowym
            var instances = await _timelineService.GetTimelineForUserAsync(userId, from, to);

            if (instances == null || !instances.Any())
            {
                return NotFound("Brak aktywności w wybranym przedziale czasowym.");
            }

            return Ok(instances);
        }

    }
}
