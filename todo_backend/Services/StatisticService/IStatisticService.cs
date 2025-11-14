using todo_backend.Dtos.Statistics;

namespace todo_backend.Services.StatisticsService
{
    public interface IStatisticService
    {
        Task<IEnumerable<StatisticsDto>> GetUserStatistics(int userId, DateTime start, DateTime end);
    }

}
