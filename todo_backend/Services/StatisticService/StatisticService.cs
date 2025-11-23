using Microsoft.EntityFrameworkCore;
using todo_backend.Data;
using todo_backend.Dtos.Statistics;
using todo_backend.Services.ActivityRecurrenceRuleService;
using todo_backend.Services.TimelineService;

namespace todo_backend.Services.StatisticsService
{
    public class StatisticService : IStatisticService
    {

        private readonly AppDbContext _context;
        private readonly ITimelineService _timelineService;

        public StatisticService(AppDbContext context, ITimelineService timelineService)
        {
            _context = context;
            _timelineService = timelineService;
        }

        public async Task<IEnumerable<StatisticsDto>> GetUserStatistics(int userId, DateTime start, DateTime end)
        {
            // Ograniczenie daty końcowej do chwili obecnej
            var validEnd = end > DateTime.UtcNow ? DateTime.UtcNow : end;

            // Pobieramy instancje przypisane DO KONKRETNEGO UŻYTKOWNIKA
            var activityInstances = await _context.ActivityInstances
                .Where(i => i.UserId == userId)
                .Where(i => i.OccurrenceDate >= start.Date && i.OccurrenceDate <= validEnd.Date)
                .Where(i => i.DidOccur)
                .Include(i => i.Activity)
                .Include(i => i.Activity.Category)
                .ToListAsync();

            // Agregacja wyników po kategoriach
            var categoryStats = activityInstances
                .GroupBy(i => i.Activity.CategoryId)
                .Select(g => new StatisticsDto
                {
                    CategoryName = g.First().Activity.Category?.Name ?? "Uncategorized",
                    TotalDurationMinutes = g.Sum(i => i.DurationMinutes),
                    InstanceCount = g.Count(),
                    ColorHex = g.First().Activity.Category?.ColorHex ?? "#999999"
                })
                .Where(stat => stat.InstanceCount > 0)
                .ToList();

            return categoryStats;
        }
    }
}
