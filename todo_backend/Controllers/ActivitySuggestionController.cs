using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using todo_backend.Dtos.ActivitySuggestionDto;
using todo_backend.Models;
using todo_backend.Services.ActivitySuggestionService;

namespace todo_backend.Controllers
{
    public class ActivitySuggestionController : ControllerBase
    {
        private readonly IActivitySuggestionService _activitySuggestionService;

        public ActivitySuggestionController(IActivitySuggestionService activitySuggestionService) {
        
            _activitySuggestionService = activitySuggestionService;
        }


        [Authorize]
        [HttpGet("suggestions/personal")]
        public async Task<IActionResult> GetPersonalSuggestions(ActivitySuggestionDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _activitySuggestionService.SuggestActivitiesAsync(userId, dto);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("suggestions/placement")]
        public async Task<IActionResult> SuggestPlacement([FromBody] ActivityPlacementSuggestionDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var results = await _activitySuggestionService.SuggestActivityPlacementAsync(userId, dto);
            if (results == null) return NoContent();
            if (!results.Any())
                return NotFound("No suitable placement found.");

            return Ok(results);
        }

        [Authorize]
        [HttpPost("suggestions/shifted")]
        public async Task<IActionResult> SuggestPlacementShifted([FromBody] ActivityPlacementSuggestionDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var results = await _activitySuggestionService.SuggestActivityPlacementShiftedAsync(userId, dto);
            if (results == null) return NoContent();
            if (!results.Any())
                return NotFound("No suitable placement found.");

            return Ok(results);
        }

    }
}
