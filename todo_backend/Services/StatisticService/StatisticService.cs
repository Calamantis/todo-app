using Microsoft.EntityFrameworkCore;
using todo_backend.Data;
using todo_backend.Dtos.Statistics;

namespace todo_backend.Services.StatisticsService
{
    public class StatisticService : IStatisticService {

        private readonly AppDbContext _context;

        public StatisticService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StatisticsDto>> GenerateUserStatsAsync(int userId, DateTime periodStart, DateTime periodEnd)
        {
            var activities = await _context.TimelineActivities
                .Where(a => a.OwnerId == userId &&
                            a.Start_time >= periodStart &&
                            (a.End_time ?? a.Start_time) <= periodEnd)
                .Include(a => a.Category)
                .ToListAsync();

            var grouped = activities
                .GroupBy(a => a.Category?.Name ?? "Uncategorized")
                .Select(g => new StatisticsDto
                {
                    Category = g.Key,
                    TotalDuration = g.Sum(a => (int)(((a.End_time ?? a.Start_time) - a.Start_time).TotalMinutes)),
                    PeriodStart = periodStart,
                    PeriodEnd = periodEnd
                });

            return grouped;
        }

    }
}
