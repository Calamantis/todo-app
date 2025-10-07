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
        public async Task<ActionResult<IEnumerable<StatisticsDto>>> GetStatistics([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var stats = await _statisticsService.GenerateUserStatsAsync(userId, start, end);
            return Ok(stats);
        }

    }
}
