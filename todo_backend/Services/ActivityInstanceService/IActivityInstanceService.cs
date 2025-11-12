using todo_backend.Dtos.ActivityInstance;

namespace todo_backend.Services.ActivityInstanceService
{
    public interface IActivityInstanceService
    {
        Task<IEnumerable<ActivityInstanceDto>> GetAllInstancesByUserIdAsync(int userId);
        Task<IEnumerable<ActivityInstanceDto>> GetInstancesByActivityIdAsync(int activityId, int userId);
        //Task<IEnumerable<ActivityInstanceDto>> GetInstancesByDateRangeAsync(DateTime startDate, DateTime endDate, int userId);
        Task<ActivityInstanceDto?> CreateInstanceAsync(ActivityInstanceDto dto, int userId);
        Task<ActivityInstanceDto?> UpdateInstanceAsync(int instanceId, ActivityInstanceDto dto, int userId);
        Task<bool> DeleteInstanceAsync(int instanceId, int userId);
    }
}
