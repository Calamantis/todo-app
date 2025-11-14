using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using todo_backend.Dtos.Statistics;
using todo_backend.Services.StatisticsService;

namespace todo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticService _statisticsService;

        public StatisticsController(IStatisticService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [Authorize]
        [HttpGet("show-statistics")]
        public async Task<ActionResult<IEnumerable<StatisticsDto>>> GetStatistics(DateTime start, DateTime end)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            try
            {
                var statistics = await _statisticsService.GetUserStatistics(userId, start, end);

                if (statistics == null || !statistics.Any())
                {
                    return NotFound("No statistics found for the given period.");
                }

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
