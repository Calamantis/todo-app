using todo_backend.Dtos.ActivitySuggestionDto;

namespace todo_backend.Services.ActivitySuggestionService
{
    public interface IActivitySuggestionService
    {
        Task<IEnumerable<SuggestedTimelineActivityDto>> SuggestActivitiesAsync(int userId, ActivitySuggestionDto dto);
    }
}
