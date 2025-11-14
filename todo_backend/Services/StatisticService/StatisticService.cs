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
            // Pobranie instancji aktywności użytkownika z okresu
            var activityInstances = await _context.ActivityInstances
                .Where(i => i.Activity.OwnerId == userId && i.OccurrenceDate >= start && i.OccurrenceDate <= end)
                .Where(i => i.DidOccur) // Filtrowanie tylko aktywności, które miały miejsce
                .Include(i => i.Activity) // Pobierz również informacje o aktywności
                .Include(i => i.Activity.Category) // Pobierz kategorię aktywności
                .ToListAsync();

            // Agregacja danych po kategoriach
            var categoryStats = activityInstances
                .GroupBy(i => i.Activity.CategoryId)
                .Select(g => new StatisticsDto
                {
                    //CategoryId = g.Key ?? 1,
                    CategoryName = g.First().Activity.Category?.Name ?? "Uncategorized",
                    TotalDurationMinutes = g.Sum(i => i.DurationMinutes), // Sumowanie czasu wykonania aktywności
                    InstanceCount = g.Count() // Liczenie liczby instancji dla danej kategorii
                })
                .ToList();

            // Ogranicz do daty bieżącej, jeżeli `end` jest większy niż `DateTime.UtcNow`
            categoryStats = categoryStats
                .Where(stat => stat.InstanceCount > 0)
                .ToList();

            return categoryStats;
        }




    }
}
