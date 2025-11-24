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

            //if (instances == null || !instances.Any())
            //{
            //    return NoContent();
            //}

            return Ok(instances);
        }

        [HttpGet("user/week-pdf")]
        public async Task<IActionResult> GetWeekTimelinePdf(
        [FromQuery] int userId,
        [FromQuery] DateTime date)
        {
            // 🔐 autoryzacja jak w GetTimelineForUserAsync
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || int.Parse(userIdClaim.Value) != userId)
            {
                return Unauthorized("Użytkownik nie jest autoryzowany do tego żądania.");
            }

            var username = User.Identity?.Name ?? $"user_{userId}";

            var pdfBytes = await _timelineService.GenerateWeekTimelinePdfAsync(userId, date, username);

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                return NotFound("Brak aktywności w tym tygodniu.");
            }

            var weekStart = GetWeekStart(date);
            var weekEnd = weekStart.AddDays(7).AddSeconds(-1);
            var fileName = $"timeline_week_{weekStart:yyyyMMdd}_{weekEnd:yyyyMMdd}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }

        private DateTime GetWeekStart(DateTime date)
        {
            var dow = (int)date.DayOfWeek;
            if (dow == 0) dow = 7;
            var diff = dow - 1;
            return date.Date.AddDays(-diff);
        }

    }
}
