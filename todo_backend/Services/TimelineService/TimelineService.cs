using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using todo_backend.Data;
using todo_backend.Dtos.ActivityInstance;
using todo_backend.Models;

namespace todo_backend.Services.TimelineService
{
    public class TimelineService : ITimelineService
    {
        private readonly AppDbContext _context;

        public TimelineService(AppDbContext context)
        {
            _context = context;
        }

        // Wyświetlanie osi czasu
        public async Task<IEnumerable<ActivityInstanceDto>> GetTimelineForUserAsync(int userId, DateTime from, DateTime to)
        {
            Console.WriteLine($"[GetTimelineForUserAsync] Pobieramy instancje aktywności dla użytkownika {userId} od {from.ToString("yyyy-MM-dd")} do {to.ToString("yyyy-MM-dd")}.");

            var instances = await _context.ActivityInstances
                .Where(i => i.Activity.OwnerId == userId && i.OccurrenceDate.Date >= from.Date && i.OccurrenceDate.Date <= to.Date) // Ignorowanie godzin
                .Include(i => i.Activity)  // Włączamy aktywność
                .ThenInclude(a => a.Category)  // Włączamy kategorię aktywności (jeśli jest)
                .Select(i => new ActivityInstanceDto
                {
                    InstanceId = i.InstanceId,
                    ActivityId = i.ActivityId,
                    RecurrenceRuleId = i.RecurrenceRuleId,
                    OccurrenceDate = i.OccurrenceDate,
                    StartTime = i.StartTime,
                    EndTime = i.EndTime,
                    DurationMinutes = i.DurationMinutes,
                    IsActive = i.IsActive,
                    DidOccur = i.DidOccur,
                    IsException = i.IsException
                })
                .ToListAsync();

            Console.WriteLine($"[GetTimelineForUserAsync] Zwrócono {instances.Count} instancji.");
            return instances;
        }


        //Funkcja generująca wystąpienia w podanym terminue
        public async Task GenerateActivityInstancesAsync(int userId, DateTime from, DateTime to)
        {
            // 1. Usuń przestarzałe instancje, które wybiegają w przyszłość (dotyczy tylko rekurencyjnych)
            var futureInstances = await _context.ActivityInstances
                .Where(i => i.RecurrenceRuleId != null && i.OccurrenceDate > DateTime.UtcNow)
                .ToListAsync();

            if (futureInstances.Any())
            {
                _context.ActivityInstances.RemoveRange(futureInstances);
                await _context.SaveChangesAsync();
            }

            // 2. Parsowanie reguł rekurencyjnych
            var recurrenceRules = await _context.ActivityRecurrenceRules
                .Where(r => r.Activity.OwnerId == userId)
                .ToListAsync();

            foreach (var rule in recurrenceRules)
            {
                DateTime currentDate = rule.DateRangeStart.Date;
                DateTime endDate = rule.DateRangeEnd?.Date ?? DateTime.UtcNow.AddMonths(1).Date;

                // Przycina zakres generowania
                var genFrom = from.Date < rule.DateRangeStart ? rule.DateRangeStart : from.Date;
                var genTo = to.Date > rule.DateRangeEnd ? rule.DateRangeEnd.Value : to.Date;

                // Sprawdzamy, jakiego typu regułę mamy do obsłużenia
                switch (rule.Type)
                {
                    case "DAY":
                        await GenerateDayInstancesAsync(rule, genFrom, genTo);
                        break;

                    case "WEEK":
                        await GenerateWeekInstancesAsync(rule, genFrom, genTo);
                        break;

                    case "MONTH":
                        await GenerateMonthInstancesAsync(rule, genFrom, genTo);
                        break;

                    case "YEAR":
                        await GenerateYearInstancesAsync(rule, genFrom, genTo);
                        break;
                }
            }

            await _context.SaveChangesAsync();
        }

        // Reguły generacji instancji o type rekurencji 'DAY'
        private async Task GenerateDayInstancesAsync(ActivityRecurrenceRule rule, DateTime genFrom, DateTime genTo)
        {

            var originalStartDate = rule.DateRangeStart.Date;

            while (originalStartDate.Date < genFrom.Date)
            {
                originalStartDate = originalStartDate.AddDays(rule.Interval ?? 1);
            }

            while (originalStartDate <= genTo.Date)
            {
                await GenerateSingularInstanceAsync(rule, originalStartDate);
                originalStartDate = originalStartDate.AddDays(rule.Interval ?? 1);
            }
        }

        // Reguły generacji instancji o type rekurencji 'WEEK'
        private async Task GenerateWeekInstancesAsync(ActivityRecurrenceRule rule, DateTime genFrom, DateTime genTo)
        {
            int counter = 0;
            while (genFrom <= genTo)
            {

                var daysOfWeek = rule.DaysOfWeek.Split(',').Select(d => int.Parse(d.Trim())).ToList();
                var dayOfWeek = (int)genFrom.DayOfWeek; // 0 - Niedziela, 1 - Poniedziałek, ... 6 - Sobota
                var Interval = rule.Interval ?? 1;

                if (counter < 7)
                {
                    if (daysOfWeek.Contains(dayOfWeek))
                    {
                        await GenerateSingularInstanceAsync(rule, genFrom);
                    }

                    genFrom = genFrom.AddDays(1);
                }
                counter++;
                if(counter == 7)
                {
                    if(Interval != 1)
                    {
                        genFrom = genFrom.AddDays(7);
                    }
                    counter = 0;
                }

            }
        }

        // Reguły generacji instancji o type rekurencji 'MONTH'
        private async Task GenerateMonthInstancesAsync(ActivityRecurrenceRule rule, DateTime genFrom, DateTime genTo)
        {
            var daysOfMonth = rule.DaysOfMonth.Split(',').ToList();

            if (daysOfMonth.Contains("LAST"))
            {
                int lastDay = DateTime.DaysInMonth(genFrom.Year, genFrom.Month);
                daysOfMonth.Remove("LAST");  // Usuń "LAST"
                daysOfMonth.Add(lastDay.ToString());  // Dodaj ostatni dzień miesiąca
            }

            var interval = rule.Interval ?? 1;
            int today = DateTime.UtcNow.Date.Day;


            daysOfMonth = daysOfMonth.Where(day => int.Parse(day) >= today).ToList();
            var DayCount = daysOfMonth.Count();

            while (genFrom <= genTo)
            {
                if (DayCount > 0)
                {
                    if (daysOfMonth.Contains(genFrom.Day.ToString()) || daysOfMonth.Contains("LAST") && genFrom.Day == DateTime.DaysInMonth(genFrom.Year, genFrom.Month))
                    {
                        await GenerateSingularInstanceAsync(rule, genFrom);
                        DayCount--;
                    }
                    if (DayCount == 0)
                    {
                        daysOfMonth = rule.DaysOfMonth.Split(',').ToList();

                        if (interval != 1)
                        {
                            var temp = genFrom.AddMonths(1);

                            for (int i = 0; i < interval - 1; i++)
                            {
                                var nextMonthDayCount = DateTime.DaysInMonth(temp.Year, temp.Month);
                                genFrom = genFrom.AddDays(nextMonthDayCount);
                            }

                            genFrom = genFrom.AddDays(1);
                            DayCount = daysOfMonth.Count();
                            continue;
                        }
                    }
                    else
                    {
                        genFrom = genFrom.AddDays(1);
                    }

                }
            }
        }

        // Reguły generacji instancji o type rekurencji 'YEAR'
        private async Task GenerateYearInstancesAsync(ActivityRecurrenceRule rule, DateTime genFrom, DateTime genTo)
        {
            if (rule.DayOfYear >= genFrom || rule.DayOfYear <= genTo) 
            {
                await GenerateSingularInstanceAsync(rule, rule.DayOfYear ?? genFrom);
            }
        }

        //Generuje instancje
        private async Task GenerateSingularInstanceAsync(ActivityRecurrenceRule rule, DateTime occurrenceDate)
        {
            if (await IsExcludedAsync(rule, occurrenceDate))
            {
                Console.WriteLine($"[EXCLUDED] Pomijam {occurrenceDate:yyyy-MM-dd} dla ActivityId={rule.ActivityId}");
                return;
            }


            var existingInstance = await _context.ActivityInstances
                .FirstOrDefaultAsync(i => i.ActivityId == rule.ActivityId && i.OccurrenceDate == occurrenceDate);

            if (existingInstance == null)
            {
                var instance = new ActivityInstance
                {
                    ActivityId = rule.ActivityId,
                    RecurrenceRuleId = rule.RecurrenceRuleId,
                    OccurrenceDate = occurrenceDate,
                    StartTime = rule.StartTime,
                    EndTime = rule.EndTime,
                    DurationMinutes = rule.DurationMinutes,
                    IsActive = true,
                    DidOccur = true,
                    IsException = false // Nowa instancja nie jest wyjątkiem
                };

                _context.ActivityInstances.Add(instance);
            }
        }


        private async Task<bool> IsExcludedAsync(ActivityRecurrenceRule rule, DateTime occurrenceDate)
        {
            return await _context.InstanceExclusions.AnyAsync(ex =>
                ex.ActivityId == rule.ActivityId &&
                ex.ExcludedDate == occurrenceDate);
        }
    }
}
