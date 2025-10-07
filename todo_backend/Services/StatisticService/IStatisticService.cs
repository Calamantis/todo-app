using todo_backend.Dtos.Statistics;

namespace todo_backend.Services.StatisticsService
{
    public interface IStatisticService
    {
        Task<IEnumerable<StatisticsDto>> GenerateUserStatsAsync(int userId, DateTime periodStart, DateTime periodEnd);
    }

}
