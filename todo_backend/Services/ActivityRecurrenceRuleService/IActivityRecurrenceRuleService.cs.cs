using todo_backend.Dtos.ActivityRecurrenceRuleDto;

namespace todo_backend.Services.ActivityRecurrenceRuleService
{
    public interface IActivityRecurrenceRuleService
    {
        //Task<IEnumerable<ActivityRecurrenceRuleDto>> GetAllRecurrenceRulesAsync();
        Task<IEnumerable<ActivityRecurrenceRuleDto>> GetRecurrenceRulesByUserIdAsync(int userId);
        Task<IEnumerable<ActivityRecurrenceRuleDto>> GetRecurrenceRulesByActivityIdAsync(int activityId, int userId);
        Task<ActivityRecurrenceRuleDto?> CreateRecurrenceRuleAsync(ActivityRecurrenceRuleDto dto, int userId);
        Task<ActivityRecurrenceRuleDto?> UpdateRecurrenceRuleAsync(int userId, int recurrenceRuleId, ActivityRecurrenceRuleDto dto);
        Task<bool> DeleteRecurrenceRuleAsync(int recurrenceRuleId, int userId);
    }
}
