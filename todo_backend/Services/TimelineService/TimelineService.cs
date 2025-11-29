using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;
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
            await CopyOwnerInstancesToParticipantAsync(userId);

            var participantActivityIds = await _context.ActivityMembers
                .Where(am => am.UserId == userId
                          && am.Role == "participant"
                          && am.Status == "accepted")
                .Select(am => am.ActivityId)
                .ToListAsync();

            var instances = await _context.ActivityInstances
                .Where(i =>
                    i.UserId == userId &&                                      
                    (i.Activity.OwnerId == userId
                    || participantActivityIds.Contains(i.ActivityId)) &&
                    i.OccurrenceDate.Date >= from.Date &&
                    i.OccurrenceDate.Date <= to.Date && i.UserId == userId)
                .Include(i => i.Activity)
                .ThenInclude(a => a.Category)
                .Select(i => new ActivityInstanceDto
                {
                    InstanceId = i.InstanceId,
                    ActivityId = i.ActivityId,
                    RecurrenceRuleId = i.RecurrenceRuleId,
                    UserId = i.UserId,
                    OccurrenceDate = i.OccurrenceDate,
                    StartTime = i.StartTime,
                    EndTime = i.EndTime,
                    DurationMinutes = i.DurationMinutes,
                    IsActive = i.IsActive,
                    DidOccur = i.DidOccur,
                    IsException = i.IsException,
                    CategoryName = i.Activity.Category.Name,
                    CategoryColorHex = i.Activity.Category.ColorHex
                })
                .ToListAsync();

            return instances;
        }

        //Funkcja generująca wystąpienia w podanym terminue
        public async Task GenerateActivityInstancesAsync(int userId, DateTime from, DateTime to)
        {
            // 1. Usuń przestarzałe instancje, które wybiegają w przyszłość (dotyczy tylko rekurencyjnych)
            var futureInstances = await _context.ActivityInstances
                .Where(i => i.RecurrenceRuleId != null && i.OccurrenceDate > DateTime.UtcNow && i.UserId == userId && i.IsException == false)
                .ToListAsync();

            if (futureInstances.Any())
            {
                _context.ActivityInstances.RemoveRange(futureInstances);
                await _context.SaveChangesAsync();
            }

            // 2. Parsowanie reguł rekurencyjnych

            //Pobranie ID aktywnosci online
            var participantActivityIds = await _context.ActivityMembers
                .Where(am => am.UserId == userId
                          && am.Role == "participant"
                          && am.Status == "accepted")
                .Select(am => am.ActivityId)
                .ToListAsync();

            //Pobranie zasad rekurencji
            var recurrenceRules = await _context.ActivityRecurrenceRules
                .Where(r =>
                    // reguły aktywności użytkownika
                    r.Activity.OwnerId == userId

                    // OR reguły aktywności, gdzie user jest uczestnikiem
                    || participantActivityIds.Contains(r.ActivityId)
                )
                .ToListAsync();

            //var recurrenceRules = await _context.ActivityRecurrenceRules
            //    .Where(r => r.Activity.OwnerId == userId)
            //    .ToListAsync();


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
                        await GenerateDayInstancesAsync(rule, genFrom, genTo, userId);
                        break;

                    case "WEEK":
                        await GenerateWeekInstancesAsync(rule, genFrom, genTo, userId);
                        break;

                    case "MONTH":
                        await GenerateMonthInstancesAsync(rule, genFrom, genTo, userId);
                        break;

                    case "YEAR":
                        await GenerateYearInstancesAsync(rule, genFrom, genTo, userId);
                        break;
                }
            }

            await _context.SaveChangesAsync();
        }

        // Reguły generacji instancji o type rekurencji 'DAY'
        private async Task GenerateDayInstancesAsync(ActivityRecurrenceRule rule, DateTime genFrom, DateTime genTo, int userId)
        {

            var originalStartDate = rule.DateRangeStart.Date;

            while (originalStartDate.Date < genFrom.Date)
            {
                originalStartDate = originalStartDate.AddDays(rule.Interval ?? 1);
            }

            while (originalStartDate <= genTo.Date)
            {
                await GenerateSingularInstanceAsync(rule, originalStartDate, userId);
                originalStartDate = originalStartDate.AddDays(rule.Interval ?? 1);
            }
        }

        // Reguły generacji instancji o type rekurencji 'WEEK'
        private async Task GenerateWeekInstancesAsync(ActivityRecurrenceRule rule, DateTime genFrom, DateTime genTo, int userId)
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
                        await GenerateSingularInstanceAsync(rule, genFrom, userId);
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
        private async Task GenerateMonthInstancesAsync(ActivityRecurrenceRule rule, DateTime genFrom, DateTime genTo, int userId)
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
                        await GenerateSingularInstanceAsync(rule, genFrom, userId);
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
        private async Task GenerateYearInstancesAsync(ActivityRecurrenceRule rule, DateTime genFrom, DateTime genTo, int userId)
        {
            if (rule.DayOfYear >= genFrom || rule.DayOfYear <= genTo) 
            {
                await GenerateSingularInstanceAsync(rule, rule.DayOfYear ?? genFrom, userId);
            }
        }

        //Generuje instancje bez czasu adapcyjnego
        //private async Task GenerateSingularInstanceAsync(ActivityRecurrenceRule rule, DateTime occurrenceDate, int userId)
        //{
        //    if (await IsExcludedAsync(rule, occurrenceDate, userId))
        //    {
        //        Console.WriteLine($"[EXCLUDED] Pomijam {occurrenceDate:yyyy-MM-dd} dla ActivityId={rule.ActivityId}");
        //        return;
        //    }


        //    var existingInstance = await _context.ActivityInstances
        //        .FirstOrDefaultAsync(i => i.ActivityId == rule.ActivityId && i.OccurrenceDate == occurrenceDate);

        //    if (existingInstance == null)
        //    {
        //        var instance = new ActivityInstance
        //        {
        //            ActivityId = rule.ActivityId,
        //            RecurrenceRuleId = rule.RecurrenceRuleId,
        //            UserId = userId,
        //            OccurrenceDate = occurrenceDate,
        //            StartTime = rule.StartTime,
        //            EndTime = rule.EndTime,
        //            DurationMinutes = rule.DurationMinutes,
        //            IsActive = true,
        //            DidOccur = true,
        //            IsException = false // Nowa instancja nie jest wyjątkiem
        //        };

        //        _context.ActivityInstances.Add(instance);
        //    }
        //}



        //Generuje instancje z czasem adapcyjnym (srednia z ostatnich 20 wystapien
        private async Task GenerateSingularInstanceAsync(ActivityRecurrenceRule rule, DateTime occurrenceDate, int userId)
        {
            if (await IsExcludedAsync(rule, occurrenceDate, rule.StartTime, rule.EndTime, userId))
            {
                Console.WriteLine($"[EXCLUDED] Pomijam {occurrenceDate:yyyy-MM-dd} dla ActivityId={rule.ActivityId}");
                return;
            }

            // 1. Ostatnie 20 instancji tej aktywności dla tego użytkownika (przed tym dniem)
            var pastInstances = await _context.ActivityInstances
                .Where(i => i.ActivityId == rule.ActivityId
                         && i.UserId == userId
                         && i.OccurrenceDate < occurrenceDate
                         && i.StartTime == rule.StartTime
                         && !i.IsException)
                .OrderByDescending(i => i.OccurrenceDate)
                .ThenByDescending(i => i.StartTime)
                .Take(20)
                .ToListAsync();

            int durationMinutes;

            if (pastInstances.Any())
            {
                durationMinutes = (int)Math.Round(pastInstances.Average(i => i.DurationMinutes));
                Console.WriteLine($"[DURATION] Średnia z ostatnich {pastInstances.Count} wystąpień = {durationMinutes} min (ActivityId={rule.ActivityId}, UserId={userId}, StartTime={rule.StartTime})");
            }
            else
            {
                durationMinutes = rule.DurationMinutes;
                Console.WriteLine($"[DURATION] Brak historii, używam rule.DurationMinutes = {durationMinutes} (ActivityId={rule.ActivityId}, UserId={userId})");
            }

            // 2. Nowy EndTime = StartTime + średni Duration
            // Jeśli używasz TimeSpan:
            var endTime = rule.StartTime.Add(TimeSpan.FromMinutes(durationMinutes));
            // Jeśli masz TimeOnly zamiast TimeSpan, zamień na:
            // var endTime = rule.StartTime.AddMinutes(durationMinutes);

            // 3. Sprawdź, czy już istnieje instancja dla tego usera / aktywności / dnia
            var existingInstance = await _context.ActivityInstances
                .FirstOrDefaultAsync(i =>
                    i.ActivityId == rule.ActivityId &&
                    i.UserId == userId &&
                    i.OccurrenceDate == occurrenceDate);

            if (existingInstance == null)
            {
                var instance = new ActivityInstance
                {
                    ActivityId = rule.ActivityId,
                    RecurrenceRuleId = rule.RecurrenceRuleId,
                    UserId = userId,
                    OccurrenceDate = occurrenceDate,
                    StartTime = rule.StartTime,
                    EndTime = endTime,
                    DurationMinutes = durationMinutes,
                    IsActive = true,
                    DidOccur = true,
                    IsException = false // Nowa instancja nie jest wyjątkiem
                };

                _context.ActivityInstances.Add(instance);

                Console.WriteLine($"[CREATE] Nowa instancja: ActivityId={instance.ActivityId}, UserId={instance.UserId}, " +
                                  $"Date={instance.OccurrenceDate:yyyy-MM-dd}, {instance.StartTime}-{instance.EndTime}, Duration={instance.DurationMinutes}");
            }
            else
            {
                Console.WriteLine($"[SKIP] Instancja już istnieje dla ActivityId={rule.ActivityId}, UserId={userId}, Date={occurrenceDate:yyyy-MM-dd}");
            }
        }

        // Filtr do instancji Offline
        private async Task<bool> IsExcludedAsync(ActivityRecurrenceRule rule, DateTime occurrenceDate, TimeSpan startTime, TimeSpan endTime, int userId)
        {
            return await _context.InstanceExclusions.AnyAsync(ex =>
                ex.ActivityId == rule.ActivityId &&
                ex.ExcludedDate == occurrenceDate &&
                ex.StartTime == startTime &&
                ex.EndTime == endTime &&
                ex.UserId == userId);
        }

        //Do kopiowania instancji online
        public async Task CopyOwnerInstancesToParticipantAsync(int currentUserId)
        {
            // 1. Wszystkie membershipy tego usera (dla debug)
            var allUserMemberships = await _context.ActivityMembers
                .Where(am => am.UserId == currentUserId)
                .ToListAsync();

            Console.WriteLine($"[DEBUG] Wszystkie membershipy usera {currentUserId}: {allUserMemberships.Count}");
            foreach (var m in allUserMemberships)
            {
                Console.WriteLine($", ActivityId={m.ActivityId}, Role={m.Role}, Status={m.Status}");
            }

            // 2. Membershipy, w których jest zaakceptowanym participantem
            var participantMemberships = allUserMemberships
                .Where(am => am.Status == "accepted" && am.Role == "participant")
                .ToList();

            Console.WriteLine($"[STEP 1] Participant 'accepted' memberships: {participantMemberships.Count}");

            if (!participantMemberships.Any())
            {
                Console.WriteLine("[STEP 1] Brak zaakceptowanych membershipów jako participant. KONIEC.");
                Console.WriteLine("========== CopyOwnerInstancesToParticipantAsync END ==========\n");
                return;
            }

            // 3. Lista ID aktywności, w których user jest participantem
            var activityIds = participantMemberships
                .Select(m => m.ActivityId)
                .Distinct()
                .ToList();

            Console.WriteLine($"[STEP 2] Unikalne ActivityId z participant memberships: {string.Join(", ", activityIds)}");

            // 4. Ownerzy dla tych aktywności (z ActivityMembers, Role='owner')
            var ownerMemberships = await _context.ActivityMembers
                .Where(am => activityIds.Contains(am.ActivityId) && am.Role == "owner")
                .ToListAsync();

            Console.WriteLine($"[STEP 2] Znalezione ownerMemberships dla tych ActivityId: {ownerMemberships.Count}");
            foreach (var om in ownerMemberships)
            {
                Console.WriteLine($"    OWNER: Activi, ActivityId={om.ActivityId}, OwnerUserId={om.UserId}, Status={om.Status}");
            }

            if (!ownerMemberships.Any())
            {
                Console.WriteLine("[STEP 2] Nie znaleziono żadnych ownerów w ActivityMembers. KONIEC.");
                Console.WriteLine("========== CopyOwnerInstancesToParticipantAsync END ==========\n");
                return;
            }

            // 5. Pary (ActivityId, OwnerId) dla tego participant-a
            var activityOwnerPairs = (
                from pm in participantMemberships
                join om in ownerMemberships on pm.ActivityId equals om.ActivityId
                select new { pm.ActivityId, OwnerId = om.UserId }
            )
            .Distinct()
            .ToList();

            Console.WriteLine($"[STEP 3] Pary (ActivityId, OwnerId) dla użytkownika {currentUserId}: {activityOwnerPairs.Count}");
            foreach (var pair in activityOwnerPairs)
            {
                Console.WriteLine($"    Pair: ActivityId={pair.ActivityId}, OwnerId={pair.OwnerId}");
            }

            if (!activityOwnerPairs.Any())
            {
                Console.WriteLine("[STEP 3] Brak par (ActivityId, Owner). KONIEC.");
                Console.WriteLine("========== CopyOwnerInstancesToParticipantAsync END ==========\n");
                return;
            }

            var newInstances = new List<ActivityInstance>();

            // 6. Dla każdej pary: sprawdź instancje ownera i kopiuj
            foreach (var pair in activityOwnerPairs)
            {
                Console.WriteLine($"\n[STEP 4] Przetwarzam ActivityId={pair.ActivityId}, OwnerId={pair.OwnerId}");

                // 6.1. Instancje ownera dla danej aktywności
                var ownerInstances = await _context.ActivityInstances
                    .Where(ai => ai.ActivityId == pair.ActivityId && ai.UserId == pair.OwnerId)
                    .ToListAsync();

                Console.WriteLine($"[STEP 4]   Owner ma instancji: {ownerInstances.Count}");
                foreach (var oi in ownerInstances)
                {
                    Console.WriteLine($"      OWNER INSTANCE: InstanceId={oi.InstanceId}, ActivityId={oi.ActivityId}, UserId={oi.UserId}, " +
                                      $"Date={oi.OccurrenceDate:yyyy-MM-dd}, {oi.StartTime}-{oi.EndTime}");
                }

                if (!ownerInstances.Any())
                {
                    Console.WriteLine("[STEP 4]   Owner nie ma żadnych instancji dla tej aktywności. Pomijam.");
                    continue;
                }

                // 6.2. Instancje uczestnika dla tej samej aktywności (żeby nie dublować)
                var participantInstances = await _context.ActivityInstances
                    .Where(ai => ai.ActivityId == pair.ActivityId && ai.UserId == currentUserId)
                    .Select(ai => new { ai.InstanceId, ai.OccurrenceDate, ai.StartTime, ai.EndTime })
                    .ToListAsync();

                Console.WriteLine($"[STEP 4]   Participant ma już {participantInstances.Count} instancji dla tej aktywności.");
                foreach (var pi in participantInstances)
                {
                    Console.WriteLine($"      PARTICIPANT INSTANCE: InstanceId={pi.InstanceId}, Date={pi.OccurrenceDate:yyyy-MM-dd}, {pi.StartTime}-{pi.EndTime}");
                }

                var participantKeys = participantInstances
                    .Select(pi => (pi.OccurrenceDate, pi.StartTime, pi.EndTime))
                    .ToHashSet();

                // 6.3. Kopiujemy każdą instancję ownera, której participant jeszcze nie ma (po dacie+godzinach)
                foreach (var ownerInstance in ownerInstances)
                {
                    var key = (ownerInstance.OccurrenceDate, ownerInstance.StartTime, ownerInstance.EndTime);

                    if (participantKeys.Contains(key))
                    {
                        Console.WriteLine($"[STEP 4]   SKIP kopiowania: participant ma już instancję " +
                                          $"Date={key.OccurrenceDate:yyyy-MM-dd} {key.StartTime}-{key.EndTime}");
                        continue;
                    }

                    if (await IsExcludedForCopyAsync(ownerInstance.ActivityId, ownerInstance.OccurrenceDate, ownerInstance.StartTime, ownerInstance.EndTime, currentUserId))
                    {
                        Console.WriteLine($"[STEP 4]   SKIP kopiowania: istnieje exclusion dla ActivityId={ownerInstance.ActivityId} " +
                                          $"w dniu {ownerInstance.OccurrenceDate:yyyy-MM-dd}");
                        continue;
                    }

                    var newInstance = new ActivityInstance
                    {
                        ActivityId = ownerInstance.ActivityId,
                        RecurrenceRuleId = ownerInstance.RecurrenceRuleId,
                        UserId = currentUserId,              // <-- kluczowe: przypisujemy do uczestnika
                        OccurrenceDate = ownerInstance.OccurrenceDate,
                        StartTime = ownerInstance.StartTime,
                        EndTime = ownerInstance.EndTime,
                        DurationMinutes = ownerInstance.DurationMinutes,
                        IsActive = ownerInstance.IsActive,
                        DidOccur = true,                      // uczestnik jeszcze nie odbył
                        IsException = ownerInstance.IsException
                    };

                    newInstances.Add(newInstance);

                    Console.WriteLine($"[STEP 4]   DODAJĘ nową instancję dla participant-a {currentUserId}: " +
                                      $"ActivityId={newInstance.ActivityId}, Date={newInstance.OccurrenceDate:yyyy-MM-dd}, " +
                                      $"{newInstance.StartTime}-{newInstance.EndTime}");
                }
            }

            Console.WriteLine($"\n[STEP 5] Łącznie nowych instancji do zapisania: {newInstances.Count}");

            if (newInstances.Any())
            {
                await _context.ActivityInstances.AddRangeAsync(newInstances);
                var saved = await _context.SaveChangesAsync();
                Console.WriteLine($"[STEP 5] SaveChangesAsync zakończone. Zapisanych rekordów: {saved}");
            }
            else
            {
                Console.WriteLine("[STEP 5] Brak nowych instancji do zapisania.");
            }

            Console.WriteLine("========== CopyOwnerInstancesToParticipantAsync END ==========\n");
        }

        //Filtr do instancji Online
        private async Task<bool> IsExcludedForCopyAsync(int activityId, DateTime occurrenceDate, TimeSpan startTime, TimeSpan endTime, int userId)
        {
            var dateOnly = occurrenceDate.Date;

            var isExcluded = await _context.InstanceExclusions.AnyAsync(ex =>
                ex.ActivityId == activityId &&
                ex.ExcludedDate.Date == dateOnly &&
                ex.StartTime == startTime &&
                ex.EndTime == endTime &&
                ex.UserId == userId);

            Console.WriteLine(
                $"[EXCLUSION CHECK] ActivityId={activityId}, OccurrenceDate={occurrenceDate:O}, " +
                $"CheckedDate={dateOnly:yyyy-MM-dd}, IsExcluded={isExcluded}");

            return isExcluded;
        }


        public async Task<byte[]?> GenerateWeekTimelinePdfAsync(int userId, DateTime date, string username)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            // 1️⃣ Wyliczamy tydzień: poniedziałek–niedziela
            var dayOfWeek = (int)date.DayOfWeek; // Sunday = 0
            if (dayOfWeek == 0) dayOfWeek = 7;   // niedziela -> 7
            var diff = dayOfWeek - 1;            // poniedziałek = 1
            var weekStart = date.Date.AddDays(-diff);
            var weekEnd = weekStart.AddDays(7).AddSeconds(-1);

            // 2️⃣ Generujemy instancje i pobieramy timeline (jak w normalnym endpointzie)
            await GenerateActivityInstancesAsync(userId, weekStart, weekEnd);
            var instances = await GetTimelineForUserAsync(userId, weekStart, weekEnd);

            if (instances == null || !instances.Any())
                return null;

            // 3️⃣ Budowa PDF
            return BuildWeekTimelinePdf(username, weekStart, weekEnd, instances);
        }

        private byte[] BuildWeekTimelinePdf(
            string username,
            DateTime weekStart,
            DateTime weekEnd,
            IEnumerable<ActivityInstanceDto> instances)
        {
            var days = instances
                .OrderBy(i => i.OccurrenceDate)
                .ThenBy(i => i.StartTime)
                .GroupBy(i => i.OccurrenceDate.Date)
                .ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(col =>
                    {
                        col.Item().Text($"Weekly timeline – {username}")
                            .FontSize(16).SemiBold();

                        col.Item().Text(
                            $"{weekStart:yyyy-MM-dd} – {weekEnd:yyyy-MM-dd}")
                            .FontSize(10).FontColor(Colors.Grey.Darken2);
                    });

                    page.Content().Column(col =>
                    {
                        foreach (var dayGroup in days)
                        {
                            var dayDate = dayGroup.Key;

                            col.Item()
                               .PaddingVertical(5)
                               .BorderBottom(1)
                               .BorderColor(Colors.Grey.Lighten2)
                               .Text(dayDate.ToString("dddd, dd.MM.yyyy"))
                               .FontSize(12)
                               .SemiBold();

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(70);  // Time
                                    columns.ConstantColumn(40);  // Duration
                                    columns.RelativeColumn();    // Activity info
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Text("Time").SemiBold();
                                    header.Cell().Text("Duration").SemiBold();
                                    header.Cell().Text("Activity").SemiBold();
                                });

                                foreach (var inst in dayGroup)
                                {
                                    var startStr = inst.StartTime.ToString(@"hh\:mm");
                                    var endStr = inst.EndTime.ToString(@"hh\:mm");
                                    var durStr = $"{inst.DurationMinutes} min";

                                    table.Cell().Text($"{startStr} – {endStr}");
                                    table.Cell().Text(durStr);

                                    // jeśli dorzucisz Title/Category do DTO, możesz je tu spokojnie użyć
                                    var label = $"Activity #{inst.ActivityId}";
                                    if (inst.IsException)
                                        label += " (exception)";

                                    table.Cell().Text(label);
                                }
                            });
                        }
                    });

                    page.Footer()
                        .AlignRight()
                        .Text(txt =>
                        {
                            txt.Span("Generated: ").FontSize(8);
                            txt.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm 'UTC'")).FontSize(8);
                        });
                });
            });

            return document.GeneratePdf();
        }

    }
}
