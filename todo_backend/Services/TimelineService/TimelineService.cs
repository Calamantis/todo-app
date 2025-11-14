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

        // Funkcja do generowania instancji aktywności
        //public async Task GenerateActivityInstancesAsync(int userId, DateTime from, DateTime to)
        //{
        //    // 1. Usuń przestarzałe instancje, które wybiegają w przyszłość (dotyczy tylko rekurencyjnych)
        //    var futureInstances = await _context.ActivityInstances
        //        .Where(i => i.RecurrenceRuleId != null && i.OccurrenceDate > DateTime.UtcNow)
        //        .ToListAsync();

        //    if (futureInstances.Any())
        //    {
        //        _context.ActivityInstances.RemoveRange(futureInstances);
        //        await _context.SaveChangesAsync();
        //    }

        //    // 2. Parsowanie reguł rekurencyjnych
        //    var recurrenceRules = await _context.ActivityRecurrenceRules
        //        .Where(r => r.Activity.OwnerId == userId)
        //        .ToListAsync();

        //    foreach (var rule in recurrenceRules)
        //    {
        //        DateTime currentDate = rule.DateRangeStart;
        //        // Dopóki data nie wybiega poza okres końca reguły lub zadany okres 'to'
        //        while (currentDate <= to)
        //        {
        //            // 2.1. Sprawdzamy, czy instancja już istnieje
        //            var existingInstance = await _context.ActivityInstances
        //                .FirstOrDefaultAsync(i => i.ActivityId == rule.ActivityId && i.OccurrenceDate == currentDate);

        //            // Jeśli instancja nie istnieje, tworzysz nową
        //            if (existingInstance == null)
        //            {
        //                var instance = new ActivityInstance
        //                {
        //                    ActivityId = rule.ActivityId,
        //                    RecurrenceRuleId = rule.RecurrenceRuleId,
        //                    OccurrenceDate = currentDate,
        //                    StartTime = rule.StartTime,
        //                    EndTime = rule.EndTime,
        //                    DurationMinutes = rule.DurationMinutes,
        //                    IsActive = true,
        //                    DidOccur = false,
        //                    IsException = false // Nowa instancja nie jest wyjątkiem
        //                };

        //                _context.ActivityInstances.Add(instance);
        //            }

        //            // 2.2. Generowanie daty kolejnej instancji w zależności od typu
        //            switch (rule.Type)
        //            {
        //                case "DAY":
        //                    // Zwiększ datę o 'Interval' dni
        //                    if (rule.Interval.HasValue)
        //                        currentDate = currentDate.AddDays(rule.Interval.Value);
        //                    break;
        //                case "WEEK":
        //                    // Zwiększ datę o 1 tydzień (poniedziałek lub inny dzień z DaysOfWeek)
        //                    currentDate = currentDate.AddDays(7);
        //                    break;
        //                case "MONTH":
        //                    // Zwiększ datę o 1 miesiąc, sprawdź, czy mieści się w DaysOfMonth
        //                    currentDate = currentDate.AddMonths(1);
        //                    break;
        //                case "YEAR":
        //                    // Zwiększ datę o 1 rok
        //                    currentDate = currentDate.AddYears(1);
        //                    break;
        //            }

        //            // Jeśli przekroczono 'DateRangeEnd', przerywamy generowanie
        //            if (rule.DateRangeEnd.HasValue && currentDate > rule.DateRangeEnd.Value)
        //                break;
        //        }
        //    }

        //    await _context.SaveChangesAsync();
        //}

        //public async Task GenerateActivityInstancesAsync(int userId, DateTime from, DateTime to)
        //{
        //    // 1. Usuń przestarzałe instancje, które wybiegają w przyszłość (dotyczy tylko rekurencyjnych)
        //    var futureInstances = await _context.ActivityInstances
        //        .Where(i => i.RecurrenceRuleId != null && i.OccurrenceDate > DateTime.UtcNow)
        //        .ToListAsync();

        //    if (futureInstances.Any())
        //    {
        //        _context.ActivityInstances.RemoveRange(futureInstances);
        //        await _context.SaveChangesAsync();
        //    }

        //    // 2. Parsowanie reguł rekurencyjnych
        //    var recurrenceRules = await _context.ActivityRecurrenceRules
        //        .Where(r => r.Activity.OwnerId == userId)
        //        .ToListAsync();

        //    foreach (var rule in recurrenceRules)
        //    {
        //        // 2. Przycinamy zakresy
        //        var activityStart = rule.DateRangeStart.Date;
        //        var activityEnd = rule.DateRangeEnd?.Date ?? DateTime.UtcNow.AddMonths(1).Date;

        //        //przycinamy zakresy do granic aktywnosci
        //        var genFrom = from.Date < activityStart ? activityStart : from.Date;
        //        var genTo = to.Date > activityEnd ? activityEnd : to.Date;


        //        //Console.WriteLine($"Processing Recurrence Rule: {rule.RecurrenceRuleId} for Activity: {rule.ActivityId}");

        //        //DateTime currentDate = from;
        //        //Console.WriteLine($"Starting Date: {currentDate}");

        //        // Dopóki data nie wybiega poza okres końca reguły lub zadany okres 'to'
        //        while (genFrom <= genTo)
        //        {
        //            if (rule.Interval != null)
        //            {

        //            } else if (rule.DaysOfWeek != null) {
        //                var daysOfWeek = rule.DaysOfWeek.Split(',').Select(d => int.Parse(d.Trim())).ToList();
        //            var dayOfWeek = (int)genFrom.DayOfWeek; // 0 - Niedziela, 1 - Poniedziałek, ... 6 - Sobota


        //            Console.WriteLine($"Checking {genFrom}: DayOfWeek = {dayOfWeek}");

        //            // Jeśli `currentDate` jest w jednym z wybranych dni tygodnia
        //            if (daysOfWeek.Contains(dayOfWeek))
        //            {
        //                Console.WriteLine($"Valid date for recurrence: {genFrom}");

        //                // 2.1. Sprawdzamy, czy instancja już istnieje
        //                var existingInstance = await _context.ActivityInstances
        //                    .FirstOrDefaultAsync(i => i.ActivityId == rule.ActivityId && i.OccurrenceDate == genFrom);

        //                // Jeśli instancja nie istnieje, tworzysz nową
        //                if (existingInstance == null)
        //                {
        //                    Console.WriteLine($"Creating new instance for activity {rule.ActivityId} on {genFrom}");

        //                    var instance = new ActivityInstance
        //                    {
        //                        ActivityId = rule.ActivityId,
        //                        RecurrenceRuleId = rule.RecurrenceRuleId,
        //                        OccurrenceDate = genFrom,
        //                        StartTime = rule.StartTime,
        //                        EndTime = rule.EndTime,
        //                        DurationMinutes = rule.DurationMinutes,
        //                        IsActive = true,
        //                        DidOccur = false,
        //                        IsException = false // Nowa instancja nie jest wyjątkiem
        //                    };

        //                    _context.ActivityInstances.Add(instance);
        //                }
        //            }
        //            else
        //            {
        //               // genFrom = genFrom.AddDays(1);
        //                Console.WriteLine($"Skipping {genFrom}, it's not a valid recurrence day.");
        //            }

        //            // 2.2. Generowanie daty kolejnej instancji w zależności od typu
        //            switch (rule.Type)
        //            {
        //                case "DAY":
        //                    Console.WriteLine($"Adding {rule.Interval} days to {genFrom}");
        //                    // Zwiększ datę o 'Interval' dni
        //                    if (rule.Interval.HasValue)
        //                        genFrom = genFrom.AddDays(rule.Interval.Value);
        //                    break;
        //                case "WEEK":
        //                    //Console.WriteLine($"Adding 7 days to {currentDate}");
        //                    // Zwiększ datę o 1 tydzień (poniedziałek lub inny dzień z DaysOfWeek)
        //                    genFrom = genFrom.AddDays(1);
        //                    break;
        //                case "MONTH":
        //                    Console.WriteLine($"Adding 1 month to {genFrom}");
        //                    // Zwiększ datę o 1 miesiąc, sprawdź, czy mieści się w DaysOfMonth
        //                    genFrom = genFrom.AddMonths(1);
        //                    break;
        //                case "YEAR":
        //                    Console.WriteLine($"Adding 1 year to {genFrom}");
        //                    // Zwiększ datę o 1 rok
        //                    genFrom = genFrom.AddYears(1);
        //                    break;
        //            }

        //            // Jeśli przekroczono 'DateRangeEnd', przerywamy generowanie
        //            if (rule.DateRangeEnd.HasValue && genFrom > rule.DateRangeEnd.Value)
        //            {
        //                Console.WriteLine($"Exceeded DateRangeEnd: {rule.DateRangeEnd.Value}. Stopping generation.");
        //                break;
        //            }
        //        }
        //    }

        //    await _context.SaveChangesAsync();
        //}

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

        private async Task GenerateDayInstancesAsync(ActivityRecurrenceRule rule, DateTime genFrom, DateTime genTo)
        {

            //var originalStartDate = rule.DateRangeStart.Date;

            //while (originalStartDate.Date < genFrom.Date)
            //{
            //    originalStartDate = originalStartDate.AddDays(rule.Interval ?? 1);
            //}

            //while (originalStartDate <= genTo.Date)
            //{
            //    // Sprawdzenie czy instancja już istnieje
            //    var existingInstance = await _context.ActivityInstances
            //        .FirstOrDefaultAsync(i => i.ActivityId == rule.ActivityId && i.OccurrenceDate == genFrom);

            //    if (existingInstance == null)
            //    {
            //        var instance = new ActivityInstance
            //        {
            //            ActivityId = rule.ActivityId,
            //            RecurrenceRuleId = rule.RecurrenceRuleId,
            //            OccurrenceDate = originalStartDate,
            //            StartTime = rule.StartTime,
            //            EndTime = rule.EndTime,
            //            DurationMinutes = rule.DurationMinutes,
            //            IsActive = true,
            //            DidOccur = true,
            //            IsException = false
            //        };
            //        _context.ActivityInstances.Add(instance);
            //    }

            //    // Dodanie 'interval' dni
            //    originalStartDate = originalStartDate.AddDays(rule.Interval ?? 1);
            //}
        }

        private async Task GenerateWeekInstancesAsync(ActivityRecurrenceRule rule, DateTime genFrom, DateTime genTo)
        {
            //int counter = 0;
            //while (genFrom <= genTo)
            //{

            //    var daysOfWeek = rule.DaysOfWeek.Split(',').Select(d => int.Parse(d.Trim())).ToList();
            //    var dayOfWeek = (int)genFrom.DayOfWeek; // 0 - Niedziela, 1 - Poniedziałek, ... 6 - Sobota
            //    var Interval = rule.Interval ?? 1;

            //    if (counter < 7)
            //    {

            //        // Sprawdzenie, czy genFrom jest w jednym z wybranych dni tygodnia
            //        if (daysOfWeek.Contains(dayOfWeek))
            //        {
            //            var existingInstance = await _context.ActivityInstances
            //                .FirstOrDefaultAsync(i => i.ActivityId == rule.ActivityId && i.OccurrenceDate == genFrom);

            //            if (existingInstance == null)
            //            {
            //                var instance = new ActivityInstance
            //                {
            //                    ActivityId = rule.ActivityId,
            //                    RecurrenceRuleId = rule.RecurrenceRuleId,
            //                    OccurrenceDate = genFrom,
            //                    StartTime = rule.StartTime,
            //                    EndTime = rule.EndTime,
            //                    DurationMinutes = rule.DurationMinutes,
            //                    IsActive = true,
            //                    DidOccur = true,
            //                    IsException = false
            //                };
            //                _context.ActivityInstances.Add(instance);
            //            }
            //        }

            //        genFrom = genFrom.AddDays(1);
            //    }
            //    counter++;
            //    if(counter == 7)
            //    {
            //        if(Interval != 1)
            //        {
            //            genFrom = genFrom.AddDays(7);
            //        }
            //        counter = 0;
            //    }

            //}
        }

        private async Task GenerateMonthInstancesAsync(ActivityRecurrenceRule rule, DateTime genFrom, DateTime genTo)
        {

            //var daysOfMonth = rule.DaysOfMonth.Split(',').ToList();

            //if (daysOfMonth.Contains("LAST"))
            //{
            //    int lastDay = DateTime.DaysInMonth(genFrom.Year, genFrom.Month);
            //    daysOfMonth.Remove("LAST");  // Usuń "LAST"
            //    daysOfMonth.Add(lastDay.ToString());  // Dodaj ostatni dzień miesiąca
            //}

            //var interval = rule.Interval ?? 1;

            //int today = DateTime.UtcNow.Date.Day;

            //daysOfMonth = daysOfMonth.Where(day => int.Parse(day) >= today).ToList();

            //// Debug - pokazanie, które dni zostaną wykorzystane
            //Console.WriteLine("Filtered DaysOfMonth: " + string.Join(", ", daysOfMonth));
            //var DayCount = daysOfMonth.Count();

            //while (genFrom <= genTo)
            //{
            //    if (DayCount > 0)
            //    {
            //        if (daysOfMonth.Contains(genFrom.Day.ToString()) || daysOfMonth.Contains("LAST") && genFrom.Day == DateTime.DaysInMonth(genFrom.Year, genFrom.Month))
            //        {


            //            Console.WriteLine("[GenerateMothInstancesAsync] Generating instance for: " + genFrom.Day.ToString());
            //            var existingInstance = await _context.ActivityInstances
            //                .FirstOrDefaultAsync(i => i.ActivityId == rule.ActivityId && i.OccurrenceDate == genFrom);

            //            if (existingInstance == null)
            //            {
            //                var instance = new ActivityInstance
            //                {
            //                    ActivityId = rule.ActivityId,
            //                    RecurrenceRuleId = rule.RecurrenceRuleId,
            //                    OccurrenceDate = genFrom,
            //                    StartTime = rule.StartTime,
            //                    EndTime = rule.EndTime,
            //                    DurationMinutes = rule.DurationMinutes,
            //                    IsActive = true,
            //                    DidOccur = true,
            //                    IsException = false
            //                };
            //                _context.ActivityInstances.Add(instance);
            //                DayCount--;
            //            }
            //        }
            //        if (DayCount == 0)
            //        {
            //            daysOfMonth = rule.DaysOfMonth.Split(',').ToList();
            //            Console.WriteLine("DaysOfMonth 2ndcycle: " + string.Join(", ", daysOfMonth));
            //            if (interval != 1)
            //            {
            //                var temp = genFrom.AddMonths(1);

            //                for (int i = 0; i < interval - 1; i++)
            //                {
            //                    var nextMonthDayCount = DateTime.DaysInMonth(temp.Year, temp.Month);
            //                    genFrom = genFrom.AddDays(nextMonthDayCount);
            //                }
            //                genFrom = genFrom.AddDays(1);
            //                //genFrom = genFrom.AddMonths(interval-1);
            //                Console.WriteLine("\n\n[INTERVAL != 1, SKIPPING TO DATE = "+genFrom+" ]\n\n");

            //                DayCount = daysOfMonth.Count();
            //                continue;
            //            }
            //        } else
            //        {
            //            genFrom = genFrom.AddDays(1);
            //        }

            //    }
            //}
        }

        private async Task GenerateYearInstancesAsync(ActivityRecurrenceRule rule, DateTime genFrom, DateTime genTo)
        {
            if (rule.DayOfYear >= genFrom || rule.DayOfYear <= genTo) 
            {
                await GenerateActivityInstanceAsync(rule, rule.DayOfYear ?? genFrom);
                //var existingInstance = await _context.ActivityInstances
                //    .FirstOrDefaultAsync(i => i.ActivityId == rule.ActivityId && i.OccurrenceDate == genFrom);

                //if (existingInstance == null)
                //{
                //    var instance = new ActivityInstance
                //    {
                //        ActivityId = rule.ActivityId,
                //        RecurrenceRuleId = rule.RecurrenceRuleId,
                //        OccurrenceDate = rule.DayOfYear ?? genFrom,
                //        StartTime = rule.StartTime,
                //        EndTime = rule.EndTime,
                //        DurationMinutes = rule.DurationMinutes,
                //        IsActive = true,
                //        DidOccur = true,
                //        IsException = false
                //    };
                //    _context.ActivityInstances.Add(instance);
                //}
            }
        }

        private async Task GenerateActivityInstanceAsync(ActivityRecurrenceRule rule, DateTime occurrenceDate)
        {
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
                    DidOccur = false,
                    IsException = false // Nowa instancja nie jest wyjątkiem
                };

                _context.ActivityInstances.Add(instance);
            }
        }



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


    }
}
