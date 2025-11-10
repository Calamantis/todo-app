using todo_backend.Dtos.ActivitySuggestionDto;

namespace todo_backend.Services.ActivitySuggestionService
{
    public interface IActivitySuggestionService
    {
        Task<IEnumerable<SuggestedTimelineActivityDto>> SuggestActivitiesAsync(int userId, ActivitySuggestionDto dto);
        //Task<IEnumerable<ActivityPlacementSuggestionResultDto>> SuggestPlacementAsync(ActivityPlacementSuggestionDto dto);
        //Task<IEnumerable<ActivityPlacementSuggestionDto?>> SuggestActivityPlacementAsync(int userId, ActivityPlacementSuggestionDto dto);
        Task<IEnumerable<DayFreeSummaryDto>> SuggestActivityPlacementAsync(int userId, ActivityPlacementSuggestionDto dto);

        Task<IEnumerable<DayOverlapActivitiesDto>> SuggestActivityPlacementShiftedAsync(int userId, ActivityPlacementSuggestionDto dto);

    }
}
