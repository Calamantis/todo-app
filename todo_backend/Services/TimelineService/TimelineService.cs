using Microsoft.EntityFrameworkCore;
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

        public async Task GenerateActivityInstancesAsync(int userId, DateTime from, DateTime to)
        {
            Console.WriteLine($"[GenerateActivityInstancesAsync] Rozpoczynamy generowanie instancji dla użytkownika {userId} od {from.ToString("yyyy-MM-dd")} do {to.ToString("yyyy-MM-dd")}.");

            // Usuń przestarzałe instancje, które wybiegają w przyszłość (dotyczy tylko rekurencyjnych)
            var futureInstances = await _context.ActivityInstances
                .Where(i => i.RecurrenceRuleId != null && i.OccurrenceDate.Date > to.Date)  // Ignorowanie godziny
                .ToListAsync();

            if (futureInstances.Any())
            {
                Console.WriteLine($"[GenerateActivityInstancesAsync] Usuwanie {futureInstances.Count} przestarzałych instancji.");
                _context.ActivityInstances.RemoveRange(futureInstances);
                await _context.SaveChangesAsync();
            }

            // Parsowanie reguł rekurencyjnych
            var recurrenceRules = await _context.ActivityRecurrenceRules
                .Where(r => r.Activity.OwnerId == userId)
                .ToListAsync();

            foreach (var rule in recurrenceRules)
            {
                Console.WriteLine($"[GenerateActivityInstancesAsync] Parsowanie reguły {rule.RecurrenceRuleId} typu {rule.Type} dla aktywności {rule.ActivityId}.");

                DateTime currentDate = rule.DateRangeStart.Date; // Ustawiamy godzinę na 00:00

                // Generowanie instancji w przedziale od 'from' do 'to'
                while (currentDate <= to.Date) // Porównujemy tylko daty (bez godzin)
                {
                    // Sprawdzamy, czy instancja już istnieje
                    var existingInstance = await _context.ActivityInstances
                        .FirstOrDefaultAsync(i => i.ActivityId == rule.ActivityId && i.OccurrenceDate.Date == currentDate);

                    if (existingInstance == null)
                    {
                        Console.WriteLine($"[GenerateActivityInstancesAsync] Instancja nie istnieje. Tworzymy nową.");

                        var instance = new ActivityInstance
                        {
                            ActivityId = rule.ActivityId,
                            RecurrenceRuleId = rule.RecurrenceRuleId,
                            OccurrenceDate = currentDate,  // Ustawiamy datę z godziną 00:00
                            StartTime = rule.StartTime,
                            EndTime = rule.EndTime,
                            DurationMinutes = rule.DurationMinutes,
                            IsActive = true,
                            DidOccur = false,
                            IsException = false
                        };

                        _context.ActivityInstances.Add(instance);
                    }

                    // Generowanie kolejnej daty instancji
                    switch (rule.Type)
                    {
                        case "DAY":
                            if (rule.Interval.HasValue)
                                currentDate = currentDate.AddDays(rule.Interval.Value);
                            break;
                        case "WEEK":

                            var today = DateTime.Today.DayOfWeek;


                            // Generowanie instancji tylko w odpowiednich dniach tygodnia (poniedziałek, wtorek)
                            var daysOfWeek = rule.DaysOfWeek.Split(',') // Parse "1,2" (Pon, Wt)
                                                .Select(d => int.Parse(d.Trim())) // Konwertujemy do listy intów
                                                .ToList(); // Lista dni tygodnia (0-6)

                            foreach (day in daysOfWeek)
                            {
                                if (today != rule.DaysOfWeek)
                            }



                            // Jeśli bieżąca data to np. środa, ustawiamy datę na poniedziałek lub wtorek
                            while (!daysOfWeek.Contains((int)currentDate.DayOfWeek))
                            {
                                Console.WriteLine($"[GenerateActivityInstancesAsync] Bieżąca data: {currentDate}, przeskakujemy na najbliższy dzień z reguły.");

                                // Znajdź najbliższy dzień tygodnia z `DaysOfWeek` i ustaw datę na ten dzień
                                int currentDayOfWeek = (int)currentDate.DayOfWeek;
                                int nextValidDay = daysOfWeek
                                                    .Where(d => d > currentDayOfWeek)
                                                    .DefaultIfEmpty(daysOfWeek.Min())  // Jeśli nie ma większego dnia, wracamy do pierwszego dnia
                                                    .Min();

                                // Zwiększ datę do najbliższego dnia tygodnia
                                int daysToAdd = nextValidDay - currentDayOfWeek;
                                if (daysToAdd < 0) daysToAdd += 7;

                                currentDate = currentDate.AddDays(daysToAdd);
                            }
                            break;
                        case "MONTH":
                            currentDate = currentDate.AddMonths(1);
                            break;
                        case "YEAR":
                            currentDate = currentDate.AddYears(1);
                            break;
                    }

                    // Jeśli daty wykraczają poza dozwolony zakres
                    if (rule.DateRangeEnd.HasValue && currentDate > rule.DateRangeEnd.Value)
                        break;
                }
            }

            Console.WriteLine($"[GenerateActivityInstancesAsync] Generowanie zakończone, zapisujemy zmiany.");
            await _context.SaveChangesAsync();
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
