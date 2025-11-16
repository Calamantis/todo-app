using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using todo_backend.Data;
using todo_backend.Dtos.ActivitySuggestionDto;
using todo_backend.Models;
using todo_backend.Services.TimelineActivityService;
using todo_backend.Services.TimelineService;

namespace todo_backend.Services.ActivitySuggestionService
{
    public class ActivitySuggestionService : IActivitySuggestionService
    {
        private readonly AppDbContext _context;
        private readonly ITimelineService _timelineService;

        public ActivitySuggestionService(AppDbContext context, ITimelineService timelineService)
        {
            _context = context;
            _timelineService = timelineService;
        }
        // Opcja 1. sugerowanie aktywności na podstawie poprzednich aktywnosci uzytkownika
        public async Task<IEnumerable<SuggestedTimelineActivityDto>> SuggestActivitiesAsync(int userId, ActivitySuggestionDto dto)
        {
            var localZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

            Console.WriteLine("========== [DEBUG] START SUGGESTION ==========");
            Console.WriteLine($"UserId: {userId}");
            Console.WriteLine($"Input DTO:");
            Console.WriteLine($"  PlannedDurationMinutes: {dto.PlannedDurationMinutes}");
            Console.WriteLine($"  PreferredStart: {dto.PreferredStart}");
            Console.WriteLine($"  PreferredEnd:   {dto.PreferredEnd}");
            Console.WriteLine($"  PreferredDays:  {(dto.PreferredDays != null ? string.Join(",", dto.PreferredDays) : "null")}");
            Console.WriteLine($"  CategoryId:     {dto.CategoryId}");
            Console.WriteLine("----------------------------------------------");

            // 🔹 Okno czasowe do analizy historii i stabilności
            var windowFrom = DateTime.UtcNow.AddMonths(-2).Date;
            var windowTo = DateTime.UtcNow.AddDays(14).Date;

            // 🔹 1️⃣ Pobierz instancje aktywności użytkownika w tym oknie
            var instanceQuery = _context.ActivityInstances
                .Include(i => i.Activity)
                .ThenInclude(a => a.Category)
                .Where(i =>
                    i.UserId == userId &&
                    i.IsActive &&
                    i.OccurrenceDate >= windowFrom &&
                    i.OccurrenceDate <= windowTo);

            if (dto.CategoryId.HasValue)
            {
                instanceQuery = instanceQuery
                    .Where(i => i.Activity.CategoryId == dto.CategoryId.Value);
            }

            var instances = await instanceQuery.ToListAsync();

            Console.WriteLine($"[DEBUG] Łączna liczba instancji w oknie: {instances.Count}");

            if (!instances.Any())
            {
                Console.WriteLine("[DEBUG] Brak instancji w historii – brak sugestii.");
                Console.WriteLine("========== [DEBUG] END ==========");
                return Enumerable.Empty<SuggestedTimelineActivityDto>();
            }

            // 🔹 2️⃣ Unikalne aktywności z historii
            var activities = instances
                .Select(i => i.Activity)
                .Where(a => a != null && a.IsActive && a.OwnerId == userId)
                .DistinctBy(a => a.ActivityId)
                .ToList();

            Console.WriteLine($"[DEBUG] Unikalnych aktywności w historii: {activities.Count}");

            var suggestions = new List<SuggestedTimelineActivityDto>();

            foreach (var act in activities)
            {
                Console.WriteLine($"\n[DEBUG] Analiza aktywności: '{act.Title}' (ActivityId={act.ActivityId})");

                var actInstances = instances
                    .Where(i => i.ActivityId == act.ActivityId)
                    .OrderByDescending(i => i.OccurrenceDate)
                    .ThenByDescending(i => i.StartTime)
                    .ToList();

                if (!actInstances.Any())
                {
                    Console.WriteLine("  -> Brak instancji tej aktywności w bieżącym oknie. Pomijam.");
                    continue;
                }

                // 🔹 3️⃣ Średni DurationMinutes z historii tej aktywności
                var avgDuration = actInstances.Average(i => (double)i.DurationMinutes);

                double muDuration = 1.0;
                if (dto.PlannedDurationMinutes.HasValue)
                {
                    var diff = Math.Abs(dto.PlannedDurationMinutes.Value - avgDuration);

                    if (diff <= 15)
                        muDuration = Math.Max(0.75, 1.0 - diff * 0.016);      // delikatny spadek
                    else
                        muDuration = Math.Max(0.05, 0.75 - (diff - 15) * 0.05); // większa kara

                    muDuration = Math.Round(muDuration, 3);
                }

                // 🔹 4️⃣ Kandydackie sloty czasowe z historii
                var candidateSlots = new List<(DayOfWeek day, TimeSpan time, DateTime local)>();

                foreach (var inst in actInstances)
                {
                    // Zakładamy, że OccurrenceDate + StartTime to lokalny czas użytkownika.
                    // Jeśli jednak trzymasz w UTC, wtedy tu zrób konwersję.
                    var local = inst.OccurrenceDate.Date + inst.StartTime;

                    candidateSlots.Add((local.DayOfWeek, inst.StartTime, local));
                }

                Console.WriteLine($"  -> Candidate slots: {candidateSlots.Count}");
                foreach (var s in candidateSlots.Take(3))
                {
                    Console.WriteLine($"     - {s.local:yyyy-MM-dd HH:mm} ({s.day}, {s.time})");
                }

                if (!candidateSlots.Any())
                {
                    Console.WriteLine("  -> Brak slotów czasowych, pomijam.");
                    continue;
                }

                // 🔹 5️⃣ Czynnik stabilności: im więcej wystąpień, tym stabilniejszy nawyk
                var totalOccurrences = candidateSlots.Count;
                var stabilityFactor = Math.Min(1.0, Math.Log10(totalOccurrences + 1) / 2.0);

                // 🔹 6️⃣ μ_day / μ_time dla najlepszego slotu
                double bestMuTime = 0.0;
                double bestMuDay = 0.0;
                (DayOfWeek day, TimeSpan time, DateTime local) bestSlot = default;

                foreach (var slot in candidateSlots)
                {
                    // μ_day – preferowane dni tygodnia
                    double muDay = 1.0;
                    if (dto.PreferredDays != null && dto.PreferredDays.Count > 0)
                        muDay = dto.PreferredDays.Contains(slot.day) ? 1.0 : 0.3;

                    // μ_time – preferowany przedział godzin
                    double muTime = 1.0;
                    if (dto.PreferredStart.HasValue && dto.PreferredEnd.HasValue)
                    {
                        var startDiff = Math.Abs((slot.time - dto.PreferredStart.Value).TotalMinutes);
                        var endDiff = Math.Abs((slot.time - dto.PreferredEnd.Value).TotalMinutes);
                        var diff = Math.Min(startDiff, endDiff);

                        if (diff <= 15)
                            muTime = Math.Max(0.85, 1.0 - diff * 0.01);
                        else
                            muTime = Math.Max(0.2, 0.85 - (diff - 15) * 0.03);
                    }

                    var weighted = 0.25 * muTime + 0.15 * muDay;
                    var bestWeighted = 0.25 * bestMuTime + 0.15 * bestMuDay;

                    if (weighted > bestWeighted)
                    {
                        bestMuTime = muTime;
                        bestMuDay = muDay;
                        bestSlot = slot;
                    }
                }

                // 🔹 7️⃣ Score końcowy (jak wcześniej, z uwzględnieniem stabilności)
                double score = 0.6 * muDuration + 0.25 * bestMuTime + 0.15 * bestMuDay;
                score *= stabilityFactor;

                Console.WriteLine($"  -> Avg Duration: {avgDuration:F1} min");
                Console.WriteLine($"  -> BEST slot: {bestSlot.local:yyyy-MM-dd HH:mm} ({bestSlot.day}, {bestSlot.time})");
                Console.WriteLine($"  -> μ_duration={muDuration:F3}, μ_time={bestMuTime:F3}, μ_day={bestMuDay:F3}, stability={stabilityFactor:F3}, SCORE={score:F3}");
                Console.WriteLine("----------------------------------------------");

                suggestions.Add(new SuggestedTimelineActivityDto
                {
                    ActivityId = act.ActivityId,
                    Title = act.Title,
                    CategoryName = act.Category?.Name,
                    SuggestedDurationMinutes = (int)Math.Round(avgDuration),
                    Score = Math.Round(score, 3)
                });
            }

            Console.WriteLine("========== [DEBUG] END ==========");

            return suggestions
                .OrderByDescending(s => s.Score)
                .Take(3)
                .ToList();
        }

        //// Opcja 2.1. sugerowanie gdzie umiescic aktywność - bez modyfikacji osi czasu użytkownika
        public async Task<IEnumerable<DayFreeSummaryDto>> SuggestActivityPlacementAsync(int userId, ActivityPlacementSuggestionDto dto)
        {
            Console.WriteLine("========== [DEBUG] SUGGEST ACTIVITY PLACEMENT ==========");

            if (dto.PlannedDuration <= 0)
            {
                Console.WriteLine("[WARN] PlannedDuration <= 0, brak sensu szukać luk.");
                return Enumerable.Empty<DayFreeSummaryDto>();
            }

            // 1️⃣ Zakres analizy (domyślnie od teraz na 14 dni)
            var start = dto.StartDate ?? DateTime.UtcNow.Date;
            var end = dto.EndDate ?? start.AddDays(15);

            if (end <= start)
            {
                Console.WriteLine("[WARN] End <= Start, zwracam pusty zbiór.");
                return Enumerable.Empty<DayFreeSummaryDto>();
            }

            var plannedMinutes = dto.PlannedDuration;
            var minRequired = plannedMinutes + 20; // 10 minut przed + 10 minut po

            Console.WriteLine($"UserId: {userId}");
            Console.WriteLine($"PlannedDuration: {plannedMinutes} min, MinRequired: {minRequired} min");
            Console.WriteLine($"Zakres analizy: {start:yyyy-MM-dd HH:mm} -> {end:yyyy-MM-dd HH:mm}");

            // 2️⃣ Pobierz wszystkie wydarzenia użytkownika z osi czasu
            await _timelineService.GenerateActivityInstancesAsync(userId, start, end);
            var userTimeline = await _timelineService.GetTimelineForUserAsync(userId, start, end);

            // 3️⃣ Okno dzienne i filtr dni tygodnia
            var prefStart = dto.PreferredStart ?? TimeSpan.FromHours(6);  // 06:00
            var prefEnd = dto.PreferredEnd ?? TimeSpan.FromHours(22); // 22:00
            var onlyDays = dto.PreferredDays != null ? new HashSet<DayOfWeek>(dto.PreferredDays) : null;

            // 4️⃣ Zamiana na proste eventy z pełnym DateTime
            var events = userTimeline
                .Where(e => e.IsActive) // jeśli chcesz brać tylko aktywne
                .Select(e => new
                {
                    Start = e.OccurrenceDate.Date + e.StartTime,
                    End = e.OccurrenceDate.Date + e.EndTime
                })
                .Where(e => e.End > start && e.Start < end) // tylko te, które w ogóle nachodzą na badany przedział
                .OrderBy(e => e.Start)
                .ToList();

            Console.WriteLine($"[DEBUG] Zebrano {events.Count} wydarzeń w zadanym zakresie.");

            var result = new List<DayFreeSummaryDto>();

            // 5️⃣ Iteracja dzień po dniu
            for (var day = start.Date; day <= end.Date; day = day.AddDays(1))
            {
                if (onlyDays != null && !onlyDays.Contains(day.DayOfWeek))
                    continue;

                var windowStart = day + prefStart;
                var windowEnd = day + prefEnd;

                // przycięcie do globalnego zakresu
                if (windowEnd <= start || windowStart >= end)
                    continue;
                if (windowStart < start) windowStart = start;
                if (windowEnd > end) windowEnd = end;

                // 6️⃣ Zdarzenia nachodzące na dzienne okno
                var overlaps = events
                    .Where(e => e.End > windowStart && e.Start < windowEnd)
                    .OrderBy(e => e.Start)
                    .ToList();

                // Zbieranie luk czasowych
                var gaps = new List<(DateTime s, DateTime e)>();

                if (!overlaps.Any())
                {
                    // cały dzień (w zadanym oknie) jest wolny
                    gaps.Add((windowStart, windowEnd));
                }
                else
                {
                    // luka przed pierwszym wydarzeniem
                    if (overlaps[0].Start > windowStart)
                        gaps.Add((windowStart, overlaps[0].Start));

                    // luki pomiędzy wydarzeniami
                    for (int i = 0; i < overlaps.Count - 1; i++)
                    {
                        var gs = overlaps[i].End;
                        var ge = overlaps[i + 1].Start;
                        if (ge > gs)
                            gaps.Add((gs, ge));
                    }

                    // luka po ostatnim wydarzeniu
                    if (overlaps[^1].End < windowEnd)
                        gaps.Add((overlaps[^1].End, windowEnd));
                }

                // 7️⃣ Dla każdej luki sprawdź, czy zmieści się PlannedDuration + 20
                foreach (var (gs, ge) in gaps)
                {
                    var gapMinutes = (int)(ge - gs).TotalMinutes;
                    if (gapMinutes < minRequired)
                        continue;

                    var slack = gapMinutes - plannedMinutes;
                    var pad = slack / 2; // wyśrodkowanie
                    var sugStart = gs.AddMinutes(pad);
                    var sugEnd = sugStart.AddMinutes(plannedMinutes);

                    result.Add(new DayFreeSummaryDto
                    {
                        DateLocal = day,
                        TotalFreeMinutes = gapMinutes,
                        SuggestedStart = sugStart.TimeOfDay,
                        SuggestedEnd = sugEnd.TimeOfDay
                    });

                    Console.WriteLine(
                        $"[SUGGEST] {day:yyyy-MM-dd}: luka {gapMinutes} min, sugeruję {sugStart:HH:mm}-{sugEnd:HH:mm}");
                }
            }

            Console.WriteLine("========== [DEBUG] SUGGEST ACTIVITY PLACEMENT END ==========");
            return result;
        }

        ////// Opcja 2.2 sugerowanie gdzie umiescic aktywność - z modyfikację osi czasu użytkownika
        public async Task<IEnumerable<DayOverlapActivitiesDto>> SuggestActivityPlacementShiftedAsync(
            int userId,
            ActivityPlacementSuggestionDto dto)
        {
            const int MAX_SHORTER_BY = 20; // luka może być max 20 min krótsza niż planowana aktywność

            var activityTime = dto.PlannedDuration;
            if (activityTime <= 0)
                return Enumerable.Empty<DayOverlapActivitiesDto>();

            // 1️⃣ Zakres analizy
            var start = dto.StartDate ?? DateTime.UtcNow;
            var end = dto.EndDate ?? start.AddDays(14);

            if (end <= start)
                return Enumerable.Empty<DayOverlapActivitiesDto>();

            // 2️⃣ Pobierz instancje z osi czasu
            await _timelineService.GenerateActivityInstancesAsync(userId, start, end);
            var userTimeline = await _timelineService.GetTimelineForUserAsync(userId, start, end);

            // 3️⃣ Preferencje dobowego okna
            var prefStart = dto.PreferredStart ?? TimeSpan.FromHours(6);   // 06:00
            var prefEnd = dto.PreferredEnd ?? TimeSpan.FromHours(22);  // 22:00
            var onlyDays = dto.PreferredDays != null ? new HashSet<DayOfWeek>(dto.PreferredDays) : null;

            var suggestions = new List<DayOverlapActivitiesDto>();

            // 4️⃣ Iteracja dzień po dniu
            for (var day = start.Date; day <= end.Date; day = day.AddDays(1))
            {
                if (onlyDays != null && !onlyDays.Contains(day.DayOfWeek))
                    continue;

                var windowStart = day + prefStart;
                var windowEnd = day + prefEnd;
                if (windowEnd <= windowStart)
                    continue;

                // przycięcie do globalnego zakresu
                if (windowEnd <= start || windowStart >= end)
                    continue;
                if (windowStart < start) windowStart = start;
                if (windowEnd > end) windowEnd = end;

                // 4.1️⃣ wszystkie eventy z tego dnia – pełny dzień
                var dayEvents = userTimeline
                    .Where(e => e.IsActive && e.OccurrenceDate.Date == day)
                    .Select(e => new
                    {
                        e.ActivityId,
                        Start = e.OccurrenceDate.Date + e.StartTime,
                        End = e.OccurrenceDate.Date + e.EndTime
                    })
                    .OrderBy(ev => ev.Start)
                    .ToList();

                // 4.2️⃣ eventy, które nachodzą na *dobowe okno* (06–22 itd.)
                var eventsInWindow = dayEvents
                    .Where(ev => ev.End > windowStart && ev.Start < windowEnd)
                    .OrderBy(ev => ev.Start)
                    .ToList();

                // jeśli brak eventów → jedna luka = całe okno
                var gaps = new List<(DateTime s, DateTime e)>();

                if (!eventsInWindow.Any())
                {
                    gaps.Add((windowStart, windowEnd));
                }
                else
                {
                    // luka przed pierwszym eventem
                    if (eventsInWindow[0].Start > windowStart)
                        gaps.Add((windowStart, eventsInWindow[0].Start));

                    // luki pomiędzy eventami
                    for (int i = 0; i < eventsInWindow.Count - 1; i++)
                    {
                        var gs = eventsInWindow[i].End;
                        var ge = eventsInWindow[i + 1].Start;
                        if (ge > gs)
                            gaps.Add((gs, ge));
                    }

                    // luka po ostatnim evencie
                    if (eventsInWindow[^1].End < windowEnd)
                        gaps.Add((eventsInWindow[^1].End, windowEnd));
                }

                // 4.3️⃣ interesujące nas luki: trochę za krótkie
                foreach (var (gs, ge) in gaps)
                {
                    var gapMinutes = (int)(ge - gs).TotalMinutes;
                    if (gapMinutes <= 0)
                        continue;

                    // luka musi być <= activityTime i >= activityTime - 20
                    if (gapMinutes > activityTime || gapMinutes < activityTime - MAX_SHORTER_BY)
                        continue;

                    var missing = activityTime - gapMinutes; // ile minut brakuje

                    // „centrujemy” aktywność w luce → może zacząć trochę przed i skończyć trochę po
                    var pad = (gapMinutes - activityTime) / 2.0; // będzie ujemne jeśli luka krótsza
                    var sugStart = gs.AddMinutes(pad);
                    var sugEnd = sugStart.AddMinutes(activityTime);

                    // 🔹 tu jest sedno: bierzemy poprzednią i następną z CAŁEGO dnia (dayEvents),
                    // a overlapy liczymy względem sugStart/sugEnd

                    // overlapy z tym zakresem
                    var overlapping = dayEvents
                        .Where(ev => ev.Start < sugEnd && ev.End > sugStart)
                        .ToList();

                    // poprzednia aktywność (kończy się przed sugStart)
                    var prev = dayEvents
                        .Where(ev => ev.End <= sugStart)
                        .OrderByDescending(ev => ev.End)
                        .FirstOrDefault();

                    // następna aktywność (zaczyna się po sugEnd)
                    var next = dayEvents
                        .Where(ev => ev.Start >= sugEnd)
                        .OrderBy(ev => ev.Start)
                        .FirstOrDefault();

                    var related = new List<(int ActivityId, DateTime Start, DateTime End)>();

                    related.AddRange(overlapping.Select(ev => (ev.ActivityId, ev.Start, ev.End)));

                    if (prev != null)
                        related.Add((prev.ActivityId, prev.Start, prev.End));

                    if (next != null)
                        related.Add((next.ActivityId, next.Start, next.End));

                    var distinctRelated = related
                        .Distinct()
                        .ToList();

                    var overlappingDtos = distinctRelated.Select(ev => new ActivityBasicInfoDto
                    {
                        ActivityId = ev.ActivityId,
                        Title = string.Empty,   // jak coś, dociągniesz Title osobnym zapytaniem po ActivityId
                        StartTime = ev.Start,
                        EndTime = ev.End
                    }).ToList();

                    suggestions.Add(new DayOverlapActivitiesDto
                    {
                        Date = day,
                        SuggestedStart = sugStart,
                        SuggestedEnd = sugEnd,
                        ActivityTime = activityTime,
                        GapTime = gapMinutes,
                        OverlappingActivities = overlappingDtos
                    });
                }
            }

            return suggestions;
        }


        // Opcja 3. sugerowanie aktywności na podstawie aktywności innych uzytkownikow
        public async Task<IEnumerable<SuggestedTimelineActivityDto>> SuggestActivitiesFromCommunityAsync(int userId, ActivitySuggestionDto dto)
        {
            var localZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

            Console.WriteLine("========== [DEBUG] COMMUNITY SUGGESTIONS ==========");

            // 1️⃣ Losowe 100 użytkowników z włączonym AllowDataStatistics (innych niż my)
            var userPool = await _context.Users
                .Where(u => u.AllowDataStatistics && u.UserId != userId)
                .OrderBy(r => Guid.NewGuid())
                .Take(100)
                .Select(u => u.UserId)
                .ToListAsync();

            if (!userPool.Any())
            {
                Console.WriteLine("⚠️ Brak użytkowników z włączoną zgodą na statystyki.");
                return Enumerable.Empty<SuggestedTimelineActivityDto>();
            }

            Console.WriteLine($"Wybrano {userPool.Count} użytkowników do analizy.");

            // 2️⃣ Pobierz ich aktywności (instancje) z ostatnich 2 miesięcy
            var windowFrom = DateTime.UtcNow.AddMonths(-2).Date;
            var windowTo = DateTime.UtcNow.Date;

            var instancesQuery = _context.ActivityInstances
                .Include(i => i.Activity)
                .ThenInclude(a => a.Category)
                .Where(i =>
                    userPool.Contains(i.UserId) &&
                    i.IsActive &&
                    i.DidOccur &&
                    i.OccurrenceDate >= windowFrom &&
                    i.OccurrenceDate <= windowTo);

            if (dto.CategoryId.HasValue)
            {
                instancesQuery = instancesQuery
                    .Where(i => i.Activity.CategoryId == dto.CategoryId.Value);
            }

            var allInstances = await instancesQuery.ToListAsync();

            if (!allInstances.Any())
            {
                Console.WriteLine("⚠️ Brak danych instancji do analizy.");
                return Enumerable.Empty<SuggestedTimelineActivityDto>();
            }

            Console.WriteLine($"Zebrano {allInstances.Count} instancji z community.");

            // 3️⃣ Normalizacja: zamiana dat na lokalne i ekstrakcja cech
            var activityGroups = allInstances
                .Select(i =>
                {
                    // Zakładamy, że OccurrenceDate + StartTime jest w czasie lokalnym użytkownika.
                    // Jeśli przechowujesz w UTC, tu trzeba byłoby dodać konwersję TimeZoneInfo.
                    var local = i.OccurrenceDate.Date + i.StartTime;

                    string timeSlot = local.Hour switch
                    {
                        >= 5 and < 12 => "morning",
                        >= 12 and < 18 => "afternoon",
                        _ => "evening"
                    };

                    return new
                    {
                        i.Activity.Title,
                        i.Activity.CategoryId,
                        CategoryName = i.Activity.Category?.Name ?? "Unknown",
                        DurationMinutes = i.DurationMinutes,
                        Day = local.DayOfWeek,
                        TimeSlot = timeSlot,
                        i.Activity.IsRecurring
                    };
                })
                .GroupBy(x => new
                {
                    x.Title,
                    x.CategoryId,
                    x.CategoryName,
                    x.TimeSlot,
                    x.Day,
                    x.IsRecurring
                })
                .Select(g => new
                {
                    g.Key.Title,
                    g.Key.CategoryId,
                    g.Key.CategoryName,
                    g.Key.TimeSlot,
                    g.Key.Day,
                    g.Key.IsRecurring,
                    Count = g.Count(),
                    AvgDuration = g.Average(x => (double)x.DurationMinutes)
                })
                .ToList();

            Console.WriteLine($"Zanalizowano {activityGroups.Count} różnych wzorców aktywności.");

            // 4️⃣ Oblicz zgodność z preferencjami użytkownika
            var suggestions = new List<SuggestedTimelineActivityDto>();

            foreach (var g in activityGroups)
            {
                double muDuration = 1.0;
                if (dto.PlannedDurationMinutes.HasValue)
                {
                    var diff = Math.Abs(dto.PlannedDurationMinutes.Value - g.AvgDuration);
                    // lekka "gaussowa" kara za różnicę
                    muDuration = Math.Exp(-Math.Pow(diff / 60.0, 2));
                }

                double muDay = 1.0;
                if (dto.PreferredDays != null && dto.PreferredDays.Count > 0)
                {
                    muDay = dto.PreferredDays.Contains(g.Day) ? 1.0 : 0.4;
                }

                double muTime = 1.0;
                if (dto.PreferredStart.HasValue)
                {
                    string preferredSlot = dto.PreferredStart.Value.Hours switch
                    {
                        >= 5 and < 12 => "morning",
                        >= 12 and < 18 => "afternoon",
                        _ => "evening"
                    };

                    muTime = (preferredSlot == g.TimeSlot) ? 1.0 : 0.5;
                }

                // liczba wystąpień jako waga popularności (nasyca się przy 25+)
                double popularity = Math.Min(1.0, g.Count / 25.0);

                double score = 0.5 * muDuration
                             + 0.2 * muDay
                             + 0.2 * muTime
                             + 0.1 * popularity;

                Console.WriteLine(
                    $"[COMMUNITY] '{g.Title}' ({g.CategoryName}) - Day={g.Day}, TimeSlot={g.TimeSlot}, " +
                    $"AvgDuration={g.AvgDuration:F1}, Count={g.Count}, " +
                    $"μ_dur={muDuration:F3}, μ_day={muDay:F3}, μ_time={muTime:F3}, pop={popularity:F3}, SCORE={score:F3}");

                suggestions.Add(new SuggestedTimelineActivityDto
                {
                    // tu nie mamy konkretnego ActivityId użytkownika, więc używamy CategoryId jako "id grupy"
                    ActivityId = g.CategoryId ?? 0,
                    Title = g.Title,
                    CategoryName = g.CategoryName,
                    SuggestedDurationMinutes = (int)Math.Round(g.AvgDuration),
                    Score = Math.Round(score, 3)
                });
            }

            Console.WriteLine("========== [DEBUG] COMMUNITY END ==========");

            return suggestions
                .OrderByDescending(s => s.Score)
                .Take(5)
                .ToList();
        }


        public async Task ApplyPlacementAdjustmentsAsync(int userId, ApplyPlacementAdjustmentsDto dto)
        {
            const int MIN_EVENT_DURATION = 10; // minimalna długość instancji po skróceniu

            Console.WriteLine("========== ApplyPlacementAdjustmentsAsync START ==========");
            Console.WriteLine($"[INPUT] userId={userId}");
            Console.WriteLine($"[INPUT] Date={dto.Date:yyyy-MM-dd}");
            Console.WriteLine($"[INPUT] SuggestedStart={dto.SuggestedStart:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"[INPUT] SuggestedEnd={dto.SuggestedEnd:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"[INPUT] ActivityId={dto.ActivityId}");
            Console.WriteLine($"[INPUT] ShortenPrevious={dto.ShortenPrevious}, ShortenCurrent={dto.ShortenCurrent}, ShortenNext={dto.ShortenNext}");

            if (dto.ShortenPrevious < 0 || dto.ShortenCurrent < 0 || dto.ShortenNext < 0)
                throw new ArgumentException("Wartości skróceń nie mogą być ujemne.");

            var date = dto.Date.Date;

            // 1️⃣ Instancje użytkownika w tym dniu
            var dayInstances = await _context.ActivityInstances
                .Where(i => i.UserId == userId
                            && i.IsActive
                            && i.OccurrenceDate.Date == date)
                .ToListAsync();

            Console.WriteLine($"[STEP 1] Znaleziono {dayInstances.Count} instancji w dniu {date:yyyy-MM-dd} dla userId={userId}");
            foreach (var di in dayInstances)
            {
                Console.WriteLine(
                    $"    InstanceId={di.InstanceId}, ActivityId={di.ActivityId}, RecRuleId={di.RecurrenceRuleId}, " +
                    $"OccDate={di.OccurrenceDate:yyyy-MM-dd}, {di.StartTime}-{di.EndTime}, IsException={di.IsException}");
            }

            // 2️⃣ Czas trwania sugerowanej aktywności
            var originalDuration = (int)(dto.SuggestedEnd - dto.SuggestedStart).TotalMinutes;
            Console.WriteLine($"[STEP 2] Obliczony czas trwania sugerowanej aktywności: {originalDuration} min");

            if (originalDuration <= 0)
                throw new InvalidOperationException("Nieprawidłowy czas trwania sugerowanej aktywności.");

            //var newCurrentDuration = originalDuration - dto.ShortenCurrent;
            //Console.WriteLine($"[STEP 2] newCurrentDuration (po ShortenCurrent={dto.ShortenCurrent}) = {newCurrentDuration} min");

            //if (newCurrentDuration < MIN_EVENT_DURATION)
            //    throw new InvalidOperationException("Planowana aktywność byłaby zbyt krótka po skróceniu.");

            //var leftTrim = dto.ShortenCurrent / 2;
            //var rightTrim = dto.ShortenCurrent - leftTrim;

            //var adjustedCurrentStart = dto.SuggestedStart.AddMinutes(leftTrim);
            //var adjustedCurrentEnd = dto.SuggestedEnd.AddMinutes(-rightTrim);

            //Console.WriteLine($"[STEP 2] adjustedCurrentStart={adjustedCurrentStart:yyyy-MM-dd HH:mm}, adjustedCurrentEnd={adjustedCurrentEnd:yyyy-MM-dd HH:mm}");
            //Console.WriteLine($"[STEP 2] leftTrim={leftTrim}, rightTrim={rightTrim}");

            var newCurrentDuration = originalDuration - dto.ShortenCurrent;
            Console.WriteLine($"[STEP 2] newCurrentDuration (po ShortenCurrent={dto.ShortenCurrent}) = {newCurrentDuration} min");

            if (newCurrentDuration < MIN_EVENT_DURATION)
                throw new InvalidOperationException("Planowana aktywność byłaby zbyt krótka po skróceniu.");

            var leftTrim = dto.ShortenCurrent / 2;
            var rightTrim = dto.ShortenCurrent - leftTrim;

            // 1️⃣ Najpierw obcięcie bieżącej aktywności
            var adjustedCurrentStart = dto.SuggestedStart.AddMinutes(leftTrim);
            var adjustedCurrentEnd = dto.SuggestedEnd.AddMinutes(-rightTrim);

            Console.WriteLine($"[STEP 2] adjustedCurrentStart={adjustedCurrentStart:yyyy-MM-dd HH:mm}, adjustedCurrentEnd={adjustedCurrentEnd:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"[STEP 2] leftTrim={leftTrim}, rightTrim={rightTrim}");

            // 2️⃣ Dodatkowe „wycentrowanie” względem sumy wszystkich skróceń
            var totalShift = dto.ShortenPrevious + dto.ShortenNext + dto.ShortenCurrent;
            var recenterOffset = totalShift / 2; // w minutach

            var finalCurrentStart = adjustedCurrentStart.AddMinutes(recenterOffset);
            var finalCurrentEnd = adjustedCurrentEnd.AddMinutes(recenterOffset);

            Console.WriteLine($"[STEP 2] recenterOffset={recenterOffset} min");
            Console.WriteLine($"[STEP 2] finalCurrentStart={finalCurrentStart:yyyy-MM-dd HH:mm}, finalCurrentEnd={finalCurrentEnd:yyyy-MM-dd HH:mm}");


            // 3️⃣ Pełne DateTime dla instancji z tego dnia
            var dayInstancesWithTimes = dayInstances
                .Select(i => new
                {
                    Instance = i,
                    Start = i.OccurrenceDate.Date + i.StartTime,
                    End = i.OccurrenceDate.Date + i.EndTime
                })
                .ToList();

            Console.WriteLine("[STEP 3] Szukanie poprzedniej i następnej instancji względem okna...");

            // PREV:
            // 1) najpierw coś, co kończy się przed adjustedCurrentStart
            var prevInstanceWrapper = dayInstancesWithTimes
                .Where(x => x.End <= adjustedCurrentStart)
                .OrderByDescending(x => x.End)
                .FirstOrDefault();

            if (prevInstanceWrapper != null)
            {
                Console.WriteLine("    PREV wybrany jako 'kończący się przed oknem'.");
            }
            else
            {
                // 2) jeśli nie ma – coś co nachodzi na lewą krawędź (start < start okna, end > start okna)
                prevInstanceWrapper = dayInstancesWithTimes
                    .Where(x => x.Start < adjustedCurrentStart && x.End > adjustedCurrentStart)
                    .OrderByDescending(x => x.End)
                    .FirstOrDefault();

                if (prevInstanceWrapper != null)
                    Console.WriteLine("    PREV wybrany jako 'nachodzący na lewą krawędź'.");
            }

            // NEXT:
            // 1) najpierw coś, co zaczyna się po adjustedCurrentEnd
            var nextInstanceWrapper = dayInstancesWithTimes
                .Where(x => x.Start >= adjustedCurrentEnd)
                .OrderBy(x => x.Start)
                .FirstOrDefault();

            if (nextInstanceWrapper != null)
            {
                Console.WriteLine("    NEXT wybrany jako 'zaczynający się po oknie'.");
            }
            else
            {
                // 2) jeśli nie ma – coś co nachodzi na prawą krawędź (start < end okna, end > end okna)
                nextInstanceWrapper = dayInstancesWithTimes
                    .Where(x => x.Start < adjustedCurrentEnd && x.End > adjustedCurrentEnd)
                    .OrderBy(x => x.Start)
                    .FirstOrDefault();

                if (nextInstanceWrapper != null)
                    Console.WriteLine("    NEXT wybrany jako 'nachodzący na prawą krawędź'.");
            }

            var prevInstance = prevInstanceWrapper?.Instance;
            var nextInstance = nextInstanceWrapper?.Instance;

            if (prevInstance != null)
            {
                Console.WriteLine(
                    $"    PREV: InstanceId={prevInstance.InstanceId}, ActivityId={prevInstance.ActivityId}, " +
                    $"Start={prevInstanceWrapper.Start:HH:mm}, End={prevInstanceWrapper.End:HH:mm}, " +
                    $"RecRuleId={prevInstance.RecurrenceRuleId}, IsException={prevInstance.IsException}");
            }
            else
            {
                Console.WriteLine("    PREV: brak poprzedniej instancji.");
            }

            if (nextInstance != null)
            {
                Console.WriteLine(
                    $"    NEXT: InstanceId={nextInstance.InstanceId}, ActivityId={nextInstance.ActivityId}, " +
                    $"Start={nextInstanceWrapper.Start:HH:mm}, End={nextInstanceWrapper.End:HH:mm}, " +
                    $"RecRuleId={nextInstance.RecurrenceRuleId}, IsException={nextInstance.IsException}");
            }
            else
            {
                Console.WriteLine("    NEXT: brak następnej instancji.");
            }

            // 4️⃣ Skracanie poprzedniej
            if (dto.ShortenPrevious > 0 && prevInstance != null)
            {
                Console.WriteLine($"[STEP 4] Skracanie poprzedniej instancji o {dto.ShortenPrevious} min...");

                var prevDuration = (prevInstance.EndTime - prevInstance.StartTime).TotalMinutes;
                var newPrevDuration = prevDuration - dto.ShortenPrevious;

                Console.WriteLine($"    PrevDuration={prevDuration} min -> newPrevDuration={newPrevDuration} min");

                if (newPrevDuration < MIN_EVENT_DURATION)
                    throw new InvalidOperationException("Poprzednia aktywność byłaby zbyt krótka po skróceniu.");

                prevInstance.EndTime = prevInstance.EndTime.Add(TimeSpan.FromMinutes(-dto.ShortenPrevious));
                prevInstance.DurationMinutes = (int)newPrevDuration;

                Console.WriteLine(
                    $"    UPDATED PREV: InstanceId={prevInstance.InstanceId}, NewEndTime={prevInstance.EndTime}, " +
                    $"NewDuration={prevInstance.DurationMinutes}");

                if (prevInstance.RecurrenceRuleId.HasValue)
                {
                    prevInstance.IsException = true;
                    Console.WriteLine($"    PREV marked as exception (RecurrenceRuleId={prevInstance.RecurrenceRuleId}).");
                }
            }
            else
            {
                Console.WriteLine("[STEP 4] Nie skracamy poprzedniej aktywności (ShortenPrevious=0 lub brak poprzedniej).");
            }

            // 5️⃣ Skracanie następnej
            if (dto.ShortenNext > 0 && nextInstance != null)
            {
                Console.WriteLine($"[STEP 5] Skracanie następnej instancji o {dto.ShortenNext} min...");

                var nextDuration = (nextInstance.EndTime - nextInstance.StartTime).TotalMinutes;
                var newNextDuration = nextDuration - dto.ShortenNext;

                Console.WriteLine($"    NextDuration={nextDuration} min -> newNextDuration={newNextDuration} min");

                if (newNextDuration < MIN_EVENT_DURATION)
                    throw new InvalidOperationException("Następna aktywność byłaby zbyt krótka po skróceniu.");

                nextInstance.StartTime = nextInstance.StartTime.Add(TimeSpan.FromMinutes(dto.ShortenNext));
                nextInstance.DurationMinutes = (int)newNextDuration;

                Console.WriteLine(
                    $"    UPDATED NEXT: InstanceId={nextInstance.InstanceId}, NewEndTime={nextInstance.EndTime}, " +
                    $"NewDuration={nextInstance.DurationMinutes}");

                if (nextInstance.RecurrenceRuleId.HasValue)
                {
                    nextInstance.IsException = true;
                    Console.WriteLine($"    NEXT marked as exception (RecurrenceRuleId={nextInstance.RecurrenceRuleId}).");
                }
            }
            else
            {
                Console.WriteLine("[STEP 5] Nie skracamy następnej aktywności (ShortenNext=0 lub brak następnej).");
            }

            // 6️⃣ Nowa / aktualna instancja
            var currentInstance = dayInstances
                .FirstOrDefault(i => i.ActivityId == dto.ActivityId);

            Console.WriteLine("[STEP 6] Obsługa instancji dla planowanej aktywności...");
            Console.WriteLine(currentInstance != null
                ? $"    Istnieje już instancja dla ActivityId={dto.ActivityId} w tym dniu (InstanceId={currentInstance.InstanceId})."
                : $"    Brak instancji dla ActivityId={dto.ActivityId} w tym dniu – utworzymy nową.");

            var rule = await _context.ActivityRecurrenceRules
                .FirstOrDefaultAsync(r => r.ActivityId == dto.ActivityId && r.IsActive);

            if (rule != null)
                Console.WriteLine($"    ActivityId={dto.ActivityId} ma regułę cykliczną (RecurrenceRuleId={rule.RecurrenceRuleId}).");
            else
                Console.WriteLine($"    ActivityId={dto.ActivityId} NIE ma aktywnej reguły cyklicznej.");



            if (currentInstance == null)
            {
                var newInstance = new ActivityInstance
                {
                    ActivityId = dto.ActivityId,
                    RecurrenceRuleId = rule?.RecurrenceRuleId,
                    UserId = userId,
                    OccurrenceDate = date,
                    StartTime = adjustedCurrentStart.TimeOfDay,
                    EndTime = adjustedCurrentEnd.TimeOfDay,
                    DurationMinutes = (int)newCurrentDuration,
                    IsActive = true,
                    DidOccur = false,
                    IsException = rule != null
                };

                _context.ActivityInstances.Add(newInstance);

                Console.WriteLine(
                    $"    NEW CURRENT INSTANCE: ActivityId={newInstance.ActivityId}, OccDate={newInstance.OccurrenceDate:yyyy-MM-dd}, " +
                    $"Start={newInstance.StartTime}, End={newInstance.EndTime}, Duration={newInstance.DurationMinutes}, " +
                    $"RecRuleId={newInstance.RecurrenceRuleId}, IsException={newInstance.IsException}");
            }
            else
            {
                currentInstance.StartTime = adjustedCurrentStart.TimeOfDay;
                currentInstance.EndTime = adjustedCurrentEnd.TimeOfDay;
                currentInstance.DurationMinutes = (int)newCurrentDuration;

                if (currentInstance.RecurrenceRuleId.HasValue || rule != null)
                {
                    currentInstance.RecurrenceRuleId ??= rule?.RecurrenceRuleId;
                    currentInstance.IsException = true;
                    Console.WriteLine(
                        $"    CURRENT INSTANCE marked as exception (InstanceId={currentInstance.InstanceId}, RecRuleId={currentInstance.RecurrenceRuleId}).");
                }

                currentInstance.IsActive = true;
                currentInstance.DidOccur = false;

                Console.WriteLine(
                    $"    UPDATED CURRENT INSTANCE: InstanceId={currentInstance.InstanceId}, Start={currentInstance.StartTime}, " +
                    $"End={currentInstance.EndTime}, Duration={currentInstance.DurationMinutes}, " +
                    $"RecRuleId={currentInstance.RecurrenceRuleId}, IsException={currentInstance.IsException}");
            }

            var saved = await _context.SaveChangesAsync();
            Console.WriteLine($"[STEP 7] SaveChangesAsync zakończone. Zapisanych rekordów: {saved}");
            Console.WriteLine("========== ApplyPlacementAdjustmentsAsync END ==========\n");
        }


    }
}
