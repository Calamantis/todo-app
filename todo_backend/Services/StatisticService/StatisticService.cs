using Microsoft.EntityFrameworkCore;
using todo_backend.Data;
using todo_backend.Dtos.Statistics;
using todo_backend.Services.RecurrenceService;

namespace todo_backend.Services.StatisticsService
{
    public class StatisticService : IStatisticService {

        private readonly AppDbContext _context;
        private readonly IRecurrenceService _recurrenceService;

        public StatisticService(AppDbContext context, IRecurrenceService recurrenceService)
        {
            _context = context;
            _recurrenceService = recurrenceService;
        }

        //public async Task<IEnumerable<StatisticsDto>> GenerateUserStatsAsync(int userId, DateTime periodStart, DateTime periodEnd)
        //{
        //    // 1️⃣ Pobieramy wszystkie aktywności użytkownika
        //    var activities = await _context.TimelineActivities
        //        .Include(a => a.Category)
        //        .Where(a => a.OwnerId == userId)
        //        .ToListAsync();

        //    var allInstances = new List<(string Category, DateTime Start, DateTime End, int DurationMinutes)>();

        //    foreach (var activity in activities)
        //    {
        //        // 🔹 Wyliczamy domyślny czas trwania
        //        var durationMinutes = activity.PlannedDurationMinutes > 0
        //            ? activity.PlannedDurationMinutes
        //            : (int)((activity.End_time ?? activity.Start_time).Subtract(activity.Start_time).TotalMinutes);

        //        // 🔹 Jeśli aktywność jest powtarzalna — generujemy wystąpienia w okresie
        //        if (activity.Is_recurring && !string.IsNullOrEmpty(activity.Recurrence_rule))
        //        {
        //            // liczba dni do przodu i do tyłu względem startu (dla GenerateOccurrences)
        //            var totalDays = (periodEnd - periodStart).Days + 1;

        //            // generujemy od daty startowej aktywności
        //            var occurrences = _recurrenceService.GenerateOccurrences(
        //                activity.Start_time,
        //                activity.Recurrence_rule,
        //                totalDays
        //            );

        //            foreach (var occurrence in occurrences)
        //            {
        //                if (occurrence >= periodStart && occurrence <= periodEnd)
        //                {
        //                    allInstances.Add((
        //                        Category: activity.Category?.Name ?? "Uncategorized",
        //                        Start: occurrence,
        //                        End: occurrence.AddMinutes(durationMinutes),
        //                        DurationMinutes: durationMinutes
        //                    ));
        //                }
        //            }
        //        }
        //        else
        //        {
        //            // 🔹 Jednorazowe aktywności mieszczące się w okresie
        //            if (activity.Start_time >= periodStart && (activity.End_time ?? activity.Start_time) <= periodEnd)
        //            {
        //                allInstances.Add((
        //                    Category: activity.Category?.Name ?? "Uncategorized",
        //                    Start: activity.Start_time,
        //                    End: (activity.End_time ?? activity.Start_time).AddMinutes(durationMinutes),
        //                    DurationMinutes: durationMinutes
        //                ));
        //            }
        //        }
        //    }

        //    // 2️⃣ Grupujemy według kategorii
        //    var grouped = allInstances
        //        .GroupBy(i => i.Category)
        //        .Select(g => new StatisticsDto
        //        {
        //            Category = g.Key,
        //            TotalDuration = g.Sum(x => x.DurationMinutes),
        //            PeriodStart = periodStart,
        //            PeriodEnd = periodEnd
        //        })
        //        .OrderByDescending(x => x.TotalDuration);

        //    return grouped;
        //}



        public async Task<IEnumerable<StatisticsDto>> GenerateUserStatsAsync(int userId, DateTime periodStart, DateTime periodEnd)
        {
            // 1️⃣ Pobierz aktywności użytkownika (własne)
            var ownActivities = await _context.TimelineActivities
                .Include(a => a.Category)
                .Where(a => a.OwnerId == userId)
                .ToListAsync();

            // 2️⃣ Pobierz aktywności, w których użytkownik jest uczestnikiem
            var joinedActivities = await _context.ActivityMembers
                .Where(am => am.UserId == userId && am.Status == "accepted")
                .Include(am => am.Activity)
                    .ThenInclude(a => a.Category)
                .Select(am => am.Activity)
                .ToListAsync();

            // 3️⃣ Połącz wyniki i usuń duplikaty (np. gdy właściciel = uczestnik)
            var allActivities = ownActivities
                .Concat(joinedActivities)
                .DistinctBy(a => a.ActivityId)
                .ToList();

            var allInstances = new List<(string Category, DateTime Start, DateTime End, int DurationMinutes)>();

            foreach (var activity in allActivities)
            {
                var durationMinutes = activity.PlannedDurationMinutes > 0
                    ? activity.PlannedDurationMinutes
                    : (int)((activity.End_time ?? activity.Start_time) - activity.Start_time).TotalMinutes;

                if (activity.Is_recurring && !string.IsNullOrEmpty(activity.Recurrence_rule))
                {
                    var totalDays = (periodEnd - periodStart).Days + 1;

                    var occurrences = _recurrenceService.GenerateOccurrences(
                        activity.Start_time,
                        activity.Recurrence_rule,
                        activity.Recurrence_exception,
                        totalDays,
                        activity.End_time
                    );

                    foreach (var occurrence in occurrences)
                    {
                        if (occurrence >= periodStart && occurrence <= periodEnd)
                        {
                            allInstances.Add((
                                Category: activity.Category?.Name ?? "Uncategorized",
                                Start: occurrence,
                                End: occurrence.AddMinutes(durationMinutes),
                                DurationMinutes: durationMinutes
                            ));
                        }
                    }
                }
                else
                {
                    if (activity.Start_time >= periodStart && (activity.End_time ?? activity.Start_time) <= periodEnd)
                    {
                        allInstances.Add((
                            Category: activity.Category?.Name ?? "Uncategorized",
                            Start: activity.Start_time,
                            End: (activity.End_time ?? activity.Start_time).AddMinutes(durationMinutes),
                            DurationMinutes: durationMinutes
                        ));
                    }
                }
            }

            // 4️⃣ Grupowanie po kategoriach
            var grouped = allInstances
                .GroupBy(i => i.Category)
                .Select(g => new StatisticsDto
                {
                    Category = g.Key,
                    TotalDuration = g.Sum(x => x.DurationMinutes),
                    PeriodStart = periodStart,
                    PeriodEnd = periodEnd
                })
                .OrderByDescending(x => x.TotalDuration);

            return grouped;
        }




    }
}
