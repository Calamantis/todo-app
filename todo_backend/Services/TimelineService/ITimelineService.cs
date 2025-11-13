using todo_backend.Dtos.ActivityInstance;

namespace todo_backend.Services.TimelineService
{
    public interface ITimelineService
    {
        Task GenerateActivityInstancesAsync(int userId, DateTime from, DateTime to);
        Task<IEnumerable<ActivityInstanceDto>> GetTimelineForUserAsync(int userId, DateTime from, DateTime to);
    }
}
