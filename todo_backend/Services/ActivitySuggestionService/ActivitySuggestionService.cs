using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using todo_backend.Data;
using todo_backend.Dtos.ActivitySuggestionDto;
using todo_backend.Services.RecurrenceService;
using todo_backend.Services.TimelineActivityService;

namespace todo_backend.Services.ActivitySuggestionService
{
    public class ActivitySuggestionService : IActivitySuggestionService
    {
        private readonly AppDbContext _context;
        private readonly IRecurrenceService _recurrenceService;
        private readonly ITimelineActivityService _timelineActivityService;

        public ActivitySuggestionService(AppDbContext context, IRecurrenceService recurrenceService, ITimelineActivityService timelineActivityService)
        {
            _context = context;
            _recurrenceService = recurrenceService;
            _timelineActivityService = timelineActivityService;
        }
        //// Opcja 1. sugerowanie aktywności na podstawie poprzednich aktywnosci uzytkownika
        //public async Task<IEnumerable<SuggestedTimelineActivityDto>> SuggestActivitiesAsync(int userId,ActivitySuggestionDto dto)
        //{
        //    var localZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

        //    // DEBUG: wejście użytkownika
        //    Console.WriteLine("========== [DEBUG] START SUGGESTION ==========");
        //    Console.WriteLine($"UserId: {userId}");
        //    Console.WriteLine($"Input DTO:");
        //    Console.WriteLine($"  PlannedDurationMinutes: {dto.PlannedDurationMinutes}");
        //    Console.WriteLine($"  PreferredStart: {dto.PreferredStart}");
        //    Console.WriteLine($"  PreferredEnd:   {dto.PreferredEnd}");
        //    Console.WriteLine($"  PreferredDays:  {(dto.PreferredDays != null ? string.Join(",", dto.PreferredDays) : "null")}");
        //    Console.WriteLine($"  CategoryId:     {dto.CategoryId}");
        //    Console.WriteLine("----------------------------------------------");

        //    var history = await _context.TimelineActivities
        //        .Include(a => a.Category)
        //        .Where(a => a.OwnerId == userId && a.Start_time > DateTime.UtcNow.AddMonths(-3))
        //        .ToListAsync();

        //    if (dto.CategoryId.HasValue)
        //        history = history.Where(a => a.CategoryId == dto.CategoryId.Value).ToList();

        //    var suggestions = new List<SuggestedTimelineActivityDto>();

        //    // Okno referencyjne do generowania slotów z rekurencji
        //    var occFromUtc = DateTime.UtcNow.AddMonths(-2);    // 2 m-ce wstecz
        //    var occToUtc = DateTime.UtcNow.AddDays(14);      // 2 tyg. do przodu

        //    foreach (var act in history)
        //    {
        //        // 1) μ_duration: ze średniej planowanych czasów dla tej nazwy
        //        var avgDuration = history
        //            .Where(a => a.Title == act.Title && a.PlannedDurationMinutes > 0)
        //            .DefaultIfEmpty()
        //            .Average(a => a?.PlannedDurationMinutes ?? act.PlannedDurationMinutes);

        //        double muDuration = 1.0;
        //        if (dto.PlannedDurationMinutes.HasValue)
        //        {
        //            var diff = Math.Abs(dto.PlannedDurationMinutes.Value - avgDuration);
        //            if (diff <= 15)
        //                muDuration = Math.Max(0.75, 1.0 - diff * 0.016);   // 1 → 0.75 w 15 min
        //            else
        //                muDuration = Math.Max(0.05, 0.75 - (diff - 15) * 0.05);
        //            muDuration = Math.Round(muDuration, 3);
        //        }

        //        // 2) μ_time & μ_day: na podstawie WYSTĄPIEŃ Z REKURENCJI
        //        //    (jeśli brak rekurencji – bierz pojedynczy start jako slot)
        //        var candidateSlots = new List<(DayOfWeek day, TimeSpan time, DateTime local)>();

        //        if (act.Is_recurring && !string.IsNullOrWhiteSpace(act.Recurrence_rule))
        //        {
        //            // Używamy Twojego serwisu, ale w wersji "z zakresem"
        //            // Jeśli nie masz overloadu (start, rule, from, to) → zrób go zgodnie z poprzednimi wskazówkami
        //            //var occ = _recurrenceService.GenerateOccurrences(
        //            //    act.Start_time, act.Recurrence_rule, act.Recurrence_exception, occFromUtc, occToUtc);

        //            //// Zmapuj na lokalną strefę i zostaw tylko unikalne (dzień, godzina), żeby nie liczyć 100x tego samego
        //            //foreach (var o in occ.Take(200))
        //            //{
        //            //    var loc = TimeZoneInfo.ConvertTimeFromUtc(
        //            //        DateTime.SpecifyKind(o, DateTimeKind.Utc), localZone);
        //            //    candidateSlots.Add((loc.DayOfWeek, loc.TimeOfDay, loc));
        //            //}

        //            //// DEBUG: pokaż kilka slotów
        //            //Console.WriteLine($"[DEBUG] Activity '{act.Title}' recurrence slots (sample):");
        //            //foreach (var s in candidateSlots.Take(5))
        //            //    Console.WriteLine($"   - {s.local:yyyy-MM-dd HH:mm} ({s.day}, {s.time})");
        //            //if (candidateSlots.Count == 0)
        //            //    Console.WriteLine("   - (no recurrence slots found in window)");
        //        }
        //        else
        //        {
        //            // Jednorazowa → 1 slot z realnego startu
        //            var loc = TimeZoneInfo.ConvertTimeFromUtc(
        //                DateTime.SpecifyKind(act.Start_time, DateTimeKind.Utc), localZone);
        //            candidateSlots.Add((loc.DayOfWeek, loc.TimeOfDay, loc));
        //            Console.WriteLine($"[DEBUG] Activity '{act.Title}' single slot: {loc:yyyy-MM-dd HH:mm} ({loc.DayOfWeek}, {loc.TimeOfDay})");
        //        }

        //        // Jeżeli nie mamy żadnego slotu (np. reguła poza oknem) – pomiń tę aktywność
        //        if (candidateSlots.Count == 0)
        //            continue;

        //        // Policz μ_time/μ_day dla KAŻDEGO slotu, weź najlepszy (max)
        //        double bestMuTime = 0.0;
        //        double bestMuDay = 0.0;
        //        (DayOfWeek day, TimeSpan time, DateTime local) bestSlot = default;

        //        foreach (var slot in candidateSlots)
        //        {
        //            // μ_day
        //            double muDay = 1.0;
        //            if (dto.PreferredDays != null && dto.PreferredDays.Count > 0)
        //                muDay = dto.PreferredDays.Contains(slot.day) ? 1.0 : 0.3;

        //            // μ_time
        //            double muTime = 1.0;
        //            if (dto.PreferredStart.HasValue && dto.PreferredEnd.HasValue)
        //            {
        //                var startDiff = Math.Abs((slot.time - dto.PreferredStart.Value).TotalMinutes);
        //                var endDiff = Math.Abs((slot.time - dto.PreferredEnd.Value).TotalMinutes);
        //                var diff = Math.Min(startDiff, endDiff);

        //                if (diff <= 15)
        //                    muTime = Math.Max(0.85, 1.0 - diff * 0.01);  // ±15 min ≈ 1 → 0.85
        //                else
        //                    muTime = Math.Max(0.2, 0.85 - (diff - 15) * 0.03);
        //            }

        //            // DEBUG: μ dla tego slotu
        //            Console.WriteLine($"   [slot] {slot.local:yyyy-MM-dd HH:mm} -> μ_time={muTime:F3}, μ_day={muDay:F3}");

        //            // wybieramy slot z NAJLEPSZYM łącznym wkładem do finalnego wyniku (ważone)
        //            var weighted = 0.25 * muTime + 0.15 * muDay; // wagi jak w głównym score
        //            var bestWeighted = 0.25 * bestMuTime + 0.15 * bestMuDay;

        //            if (weighted > bestWeighted)
        //            {
        //                bestMuTime = muTime;
        //                bestMuDay = muDay;
        //                bestSlot = slot;
        //            }
        //        }

        //        // Finalny wynik
        //        double score = 0.6 * muDuration + 0.25 * bestMuTime + 0.15 * bestMuDay;

        //        Console.WriteLine($"[DEBUG] Activity: {act.Title}");
        //        Console.WriteLine($"  -> Avg Duration: {avgDuration} min");
        //        Console.WriteLine($"  -> BEST slot: {bestSlot.local:yyyy-MM-dd HH:mm} ({bestSlot.day}, {bestSlot.time})");
        //        Console.WriteLine($"  -> μ_duration={muDuration:F3}, μ_time(best)={bestMuTime:F3}, μ_day(best)={bestMuDay:F3}, SCORE={score:F3}");
        //        Console.WriteLine("----------------------------------------------");

        //        suggestions.Add(new SuggestedTimelineActivityDto
        //        {
        //            ActivityId = act.ActivityId,
        //            Title = act.Title,
        //            CategoryName = act.Category?.Name,
        //            SuggestedDurationMinutes = (int)Math.Round(avgDuration),
        //            Score = Math.Round(score, 3),
        //        });
        //    }

        //    Console.WriteLine("========== [DEBUG] END ==========");

        //    return suggestions
        //        .OrderByDescending(s => s.Score)
        //        .Take(3)
        //        .ToList();
        //}

        //// Opcja 2.1. sugerowanie gdzie umiescic aktywność - bez modyfikacji osi czasu użytkownika
        //public async Task<IEnumerable<DayFreeSummaryDto>> SuggestActivityPlacementAsync(int userId, ActivityPlacementSuggestionDto dto)
        //{
        //    // 1) Aktywność i jej czas trwania
        //    var activity = await _context.TimelineActivities
        //        .FirstOrDefaultAsync(t => t.ActivityId == dto.ActivityId && t.OwnerId == userId);
        //    if (activity == null) return Enumerable.Empty<DayFreeSummaryDto>();

        //    var activityMinutes = activity.PlannedDurationMinutes; // np. 160
        //    var minRequired = activityMinutes + 20; // co najmniej 20 minut więcej

        //    // 2) Zakres analizy – bez konwersji
        //    var start = (dto.StartDate ?? DateTime.UtcNow);
        //    var end = (dto.EndDate ?? start.AddDays(14));
        //    if (end <= start) return Enumerable.Empty<DayFreeSummaryDto>();

        //    // 3) Dane osi czasu (zakładamy spójny czas z 'start'/'end')
        //    var userTimeline = await _timelineActivityService.GetTimelineForUserAsync(userId, start, end);

        //    // 4) Okno dzienne i ew. filtr dni tygodnia – bez konwersji
        //    var prefStart = dto.PreferredStart ?? TimeSpan.FromHours(6);   // 06:00
        //    var prefEnd = dto.PreferredEnd ?? TimeSpan.FromHours(22);  // 22:00
        //    var onlyDays = dto.PreferredDays != null ? new HashSet<DayOfWeek>(dto.PreferredDays) : null;

        //    // 5) Proste wydarzenia: od filtracji nulli do uporządkowanej listy
        //    var events = userTimeline
        //        .Select(e => new { Start = e.StartTime, End = e.EndTime })
        //        .Where(e => e.End > start && e.Start < end)
        //        .OrderBy(e => e.Start)
        //        .ToList();

        //    var result = new List<DayFreeSummaryDto>();

        //    // 6) Iteracja dzień po dniu (bez TZ – w tej samej skali co dane)
        //    for (var day = start.Date; day <= end.Date; day = day.AddDays(1))
        //    {
        //        if (onlyDays != null && !onlyDays.Contains(day.DayOfWeek)) continue;

        //        var windowStart = day + prefStart;
        //        var windowEnd = day + prefEnd;
        //        if (windowEnd <= windowStart) continue;

        //        // przycięcie do globalnego zakresu
        //        if (windowEnd <= start || windowStart >= end) continue;
        //        if (windowStart < start) windowStart = start;
        //        if (windowEnd > end) windowEnd = end;

        //        // Zdarzenia nachodzące okno
        //        var overlaps = events
        //            .Select(e => new
        //            {
        //                S = e.Start < windowStart ? windowStart : e.Start,
        //                E = e.End > windowEnd ? windowEnd : e.End
        //            })
        //            .Where(e => e.E > e.S)
        //            .OrderBy(e => e.S)
        //            .ToList();

        //        // Zbieramy luki
        //        var gaps = new List<(DateTime s, DateTime e)>();
        //        if (overlaps.Count == 0)
        //        {
        //            gaps.Add((windowStart, windowEnd));
        //        }
        //        else
        //        {
        //            if (overlaps[0].S > windowStart) gaps.Add((windowStart, overlaps[0].S));
        //            for (int i = 0; i < overlaps.Count - 1; i++)
        //            {
        //                var gs = overlaps[i].E;
        //                var ge = overlaps[i + 1].S;
        //                if (ge > gs) gaps.Add(((DateTime s, DateTime e))(gs, ge));
        //            }
        //            if (overlaps[overlaps.Count - 1].E < windowEnd) gaps.Add(((DateTime s, DateTime e))(overlaps[overlaps.Count - 1].E, windowEnd));
        //        }

        //        // Dla każdej luki spełniającej warunek zwróć propozycję (w połowie luki)
        //        foreach (var (gs, ge) in gaps)
        //        {
        //            var gapMinutes = (int)(ge - gs).TotalMinutes;
        //            if (gapMinutes < minRequired) continue;

        //            var slack = gapMinutes - activityMinutes;  // >= 20
        //            var pad = slack / 2;                     // wyśrodkowanie
        //            var sugStart = gs.AddMinutes(pad);
        //            var sugEnd = sugStart.AddMinutes(activityMinutes);

        //            result.Add(new DayFreeSummaryDto
        //            {
        //                DateLocal = day,                 // „dzień” tej luki
        //                TotalFreeMinutes = gapMinutes,          // długość tej luki (jeśli chcesz, zmień nazwę pola)
        //                SuggestedStart = sugStart,
        //                SuggestedEnd = sugEnd
        //            });
        //        }
        //    }

        //    return result;
        //}

        //// Opcja 2.2 sugerowanie gdzie umiescic aktywność - z modyfikację osi czasu użytkownika
        //public async Task<IEnumerable<DayOverlapActivitiesDto>> SuggestActivityPlacementShiftedAsync(int userId, ActivityPlacementSuggestionDto dto)
        //{
        //    // 1) Aktywność i jej czas trwania
        //    var activity = await _context.TimelineActivities
        //        .FirstOrDefaultAsync(t => t.ActivityId == dto.ActivityId && t.OwnerId == userId);
        //    if (activity == null) return Enumerable.Empty<DayOverlapActivitiesDto>();

        //    var activityMinutes = activity.PlannedDurationMinutes; // np. 160
        //    var minRequired = activityMinutes + 20; // co najmniej 20 minut więcej

        //    // 2) Zakres analizy – bez konwersji
        //    var start = (dto.StartDate ?? DateTime.UtcNow);
        //    var end = (dto.EndDate ?? start.AddDays(14));
        //    if (end <= start) return Enumerable.Empty<DayOverlapActivitiesDto>();

        //    // 3) Dane osi czasu (zakładamy spójny czas z 'start'/'end')
        //    var userTimeline = await _timelineActivityService.GetTimelineForUserAsync(userId, start, end);

        //    // 4) Okno dzienne i ew. filtr dni tygodnia – bez konwersji
        //    var prefStart = dto.PreferredStart ?? TimeSpan.FromHours(6);   // 06:00
        //    var prefEnd = dto.PreferredEnd ?? TimeSpan.FromHours(22);  // 22:00
        //    var onlyDays = dto.PreferredDays != null ? new HashSet<DayOfWeek>(dto.PreferredDays) : null;

        //    // 5) Proste wydarzenia: od filtracji nulli do uporządkowanej listy
        //    var events = userTimeline
        //        .Select(e => new { Start = e.StartTime, End = e.EndTime })
        //        .Where(e => e.End > start && e.Start < end)
        //        .OrderBy(e => e.Start)
        //        .ToList();

        //    var result = new List<DayFreeSummaryDto>();
        //    var finalresult = new List<DayOverlapActivitiesDto>();

        //    // 6) Iteracja dzień po dniu (bez TZ – w tej samej skali co dane)
        //    for (var day = start.Date; day <= end.Date; day = day.AddDays(1))
        //    {
        //        if (onlyDays != null && !onlyDays.Contains(day.DayOfWeek)) continue;

        //        var windowStart = day + prefStart;
        //        var windowEnd = day + prefEnd;
        //        if (windowEnd <= windowStart) continue;

        //        // Przycięcie do globalnego zakresu
        //        if (windowEnd <= start || windowStart >= end) continue;
        //        if (windowStart < start) windowStart = start;
        //        if (windowEnd > end) windowEnd = end;

        //        // Zdarzenia nachodzące okno
        //        var overlaps = events
        //            .Select(e => new
        //            {
        //                S = e.Start < windowStart ? windowStart : e.Start,
        //                E = e.End > windowEnd ? windowEnd : e.End
        //            })
        //            .Where(e => e.E > e.S)
        //            .OrderBy(e => e.S)
        //            .ToList();

        //        // Zbieramy luki
        //        var gaps = new List<(DateTime s, DateTime e)>();

        //        if (overlaps.Count == 0)
        //        {
        //            gaps.Add((windowStart, windowEnd));
        //        }
        //        else
        //        {
        //            if (overlaps[0].S > windowStart) gaps.Add((windowStart, overlaps[0].S));
        //            for (int i = 0; i < overlaps.Count - 1; i++)
        //            {
        //                var gs = overlaps[i].E;
        //                var ge = overlaps[i + 1].S;
        //                if (ge > gs) gaps.Add(((DateTime s, DateTime e))(gs, ge));  // dodajemy tylko, jeśli są różne
        //            }
        //            if (overlaps[overlaps.Count - 1].E < windowEnd) gaps.Add(((DateTime s, DateTime e))(overlaps[overlaps.Count - 1].E, windowEnd));
        //        }

        //        foreach (var (gs, ge) in gaps)
        //        {
        //            var gapMinutes = (int)(ge - gs).TotalMinutes;

        //            // tylko luki lekko za krótkie (np. 150–179 minut przy aktywności 180)
        //            if (gapMinutes <= activityMinutes && gapMinutes >= activityMinutes - 30)
        //            {
        //                var slack = gapMinutes - activityMinutes;  // np. -20
        //                var pad = slack / 2;                       // "środek" luki
        //                var sugStart = gs.AddMinutes(pad);
        //                var sugEnd = sugStart.AddMinutes(activityMinutes);

        //                Console.WriteLine("-------------------------------------------------");
        //                Console.WriteLine($"🕓 Luka dnia: {day:yyyy-MM-dd}");
        //                Console.WriteLine($"Długość luki: {gapMinutes} min");
        //                Console.WriteLine($"Czas aktywności: {activityMinutes} min");
        //                Console.WriteLine($"Start sugerowany: {sugStart:HH:mm}");
        //                Console.WriteLine($"Koniec sugerowany: {sugEnd:HH:mm}");
        //                Console.WriteLine("-------------------------------------------------\n");


        //                result.Add(new DayFreeSummaryDto
        //                {
        //                    DateLocal = day,
        //                    TotalFreeMinutes = gapMinutes,
        //                    SuggestedStart = sugStart,
        //                    SuggestedEnd = sugEnd
        //                });
        //            }
        //        }


        //        finalresult = await GetActivitiesOverlappingSuggestionsAsync(userId, result, activityMinutes, dto.ActivityId);

        //    }
        //    return finalresult;
        //}

        //// 2.2 kontynuacja
        //public async Task<List<DayOverlapActivitiesDto>> GetActivitiesOverlappingSuggestionsAsync(int userId, List<DayFreeSummaryDto> suggestedSlots, int activityTime, int activityId)
        //{
        //    var overlapsPerSlot = new List<DayOverlapActivitiesDto>();

        //    // pobierz pełny timeline użytkownika tylko raz (dla zakresu całego okresu)
        //    var minStart = suggestedSlots.Min(s => s.SuggestedStart) ?? DateTime.UtcNow;
        //    var maxEnd = suggestedSlots.Max(s => s.SuggestedEnd) ?? minStart.AddDays(14);

        //    var userTimeline = await _timelineActivityService.GetTimelineForUserAsync(userId, minStart, maxEnd);

        //    if (userTimeline == null || !userTimeline.Any())
        //    {
        //        Console.WriteLine("Brak aktywności w osi czasu użytkownika.");
        //        return overlapsPerSlot;
        //    }

        //    foreach (var slot in suggestedSlots)
        //    {
        //        var slotStart = slot.SuggestedStart ?? DateTime.MinValue;
        //        var slotEnd = slot.SuggestedEnd ?? DateTime.MinValue;

        //        //// znajdź aktywności, które trwają w tym zakresie
        //        //var overlapping = userTimeline
        //        //    .Where(a =>
        //        //        a.StartTime <= slotEnd &&   // zaczyna się przed końcem
        //        //        a.EndTime >= slotStart      // kończy się po rozpoczęciu
        //        //    )
        //        //    .OrderBy(a => a.StartTime)
        //        //    .ToList();

        //        //if (overlapping.Any())
        //        //{
        //        //    overlapsPerSlot.Add(new DayOverlapActivitiesDto
        //        //    {
        //        //        Date = slot.DateLocal,
        //        //        SuggestedStart = slotStart,
        //        //        SuggestedEnd = slotEnd,
        //        //        gapTime = slot.TotalFreeMinutes,
        //        //        activityTime = activityTime,
        //        //        OverlappingActivities = overlapping.Select(a => new ActivityBasicInfoDto
        //        //        {
        //        //            ActivityId = a.ActivityId,
        //        //            Title = a.Title,
        //        //            StartTime = a.StartTime,
        //        //            EndTime = a.EndTime
        //        //        }).ToList()
        //        //    });
        //        //}


        //        // znajdź aktywności, które trwają w tym zakresie
        //        var overlapping = userTimeline
        //            .Where(a =>
        //                a.StartTime <= slotEnd &&   // zaczyna się przed końcem
        //                a.EndTime >= slotStart      // kończy się po rozpoczęciu
        //            )
        //            .OrderBy(a => a.StartTime)
        //            .ToList();

        //        // znajdź poprzednią i następną aktywność względem luki
        //        var prevActivity = userTimeline
        //            .Where(a => a.EndTime <= slotStart)
        //            .OrderByDescending(a => a.EndTime)
        //            .FirstOrDefault();

        //        var nextActivity = userTimeline
        //            .Where(a => a.StartTime >= slotEnd)
        //            .OrderBy(a => a.StartTime)
        //            .FirstOrDefault();

        //        if (overlapping.Any() || prevActivity != null || nextActivity != null)
        //        {
        //            var modificationProposals = new List<ActivityModificationSuggestionDto>();

        //            // --- 🔹 1. Skrócenie poprzedniej aktywności ---
        //            if (prevActivity != null)
        //            {
        //                // jak bardzo można skrócić (min 5, max 15)
        //                var maxShorten = Math.Min(15, (slotStart - prevActivity.EndTime!.Value).TotalMinutes + 15);
        //                if (maxShorten >= 5)
        //                {
        //                    var newEnd = prevActivity.EndTime!.Value.AddMinutes(-maxShorten);
        //                    modificationProposals.Add(new ActivityModificationSuggestionDto
        //                    {
        //                        ActivityId = prevActivity.ActivityId,
        //                        ModificationType = "ShortenPrevious",
        //                        Description = $"Skróć poprzednią aktywność o {maxShorten:F0} min, aby uzyskać przerwę ≥5 min.",
        //                        NewStartTime = prevActivity.StartTime,
        //                        NewEndTime = newEnd
        //                    });
        //                }
        //            }

        //            // --- 🔹 2. Przesunięcie następnej aktywności ---
        //            if (nextActivity != null)
        //            {
        //                var maxShift = Math.Min(15, (nextActivity.StartTime - slotEnd).TotalMinutes + 15);
        //                if (maxShift >= 5)
        //                {
        //                    var newStart = nextActivity.StartTime.AddMinutes(maxShift);
        //                    var newEnd = nextActivity.EndTime!.Value.AddMinutes(maxShift);

        //                    modificationProposals.Add(new ActivityModificationSuggestionDto
        //                    {
        //                        ActivityId = nextActivity.ActivityId,
        //                        ModificationType = "ShiftNext",
        //                        Description = $"Przesuń następną aktywność o {maxShift:F0} min, aby uzyskać przerwę ≥5 min.",
        //                        NewStartTime = newStart,
        //                        NewEndTime = newEnd
        //                    });
        //                }
        //            }

        //            // --- 🔹 3. Skrócenie bieżącej (proponowanej) aktywności ---
        //            if (overlapping.Any())
        //            {
        //                var totalOverlap = overlapping.Sum(a =>
        //                {
        //                    var start = a.StartTime;
        //                    var end = a.EndTime;

        //                    // sprawdzenie dla nulli, żeby uniknąć problemów
        //                    if (start == null || end == null) return 0;

        //                    var overlapStart = start > slotStart ? start : slotStart;
        //                    var overlapEnd = end < slotEnd ? end.Value : slotEnd;

        //                    var overlapMinutes = (overlapEnd - overlapStart).TotalMinutes;
        //                    return overlapMinutes > 0 ? overlapMinutes : 0;
        //                });

        //                if (totalOverlap >= 0 && totalOverlap <= 20)
        //                {
        //                    // skróć z każdej strony tyle, żeby zostawić min. 5 minut przerwy
        //                    var shortenLeft = Math.Min(10, (totalOverlap + 10) / 2);
        //                    var shortenRight = Math.Min(10, (totalOverlap + 10) - shortenLeft);

        //                    var newStart = slotStart.AddMinutes(shortenLeft);
        //                    var newEnd = slotEnd.AddMinutes(-shortenRight);

        //                    modificationProposals.Add(new ActivityModificationSuggestionDto
        //                    {
        //                        ActivityId = activityId, // bieżąca (nowa) aktywność
        //                        ModificationType = "ShortenCurrent",
        //                        Description = $"Skróć planowaną aktywność o {shortenLeft + shortenRight:F0} min, aby uzyskać odstęp ≥5 min z obu stron.",
        //                        NewStartTime = newStart,
        //                        NewEndTime = newEnd
        //                    });
        //                }
        //            }

        //            overlapsPerSlot.Add(new DayOverlapActivitiesDto
        //            {
        //                Date = slot.DateLocal,
        //                SuggestedStart = slotStart,
        //                SuggestedEnd = slotEnd,
        //                gapTime = slot.TotalFreeMinutes,
        //                activityTime = activityTime,
        //                OverlappingActivities = overlapping.Select(a => new ActivityBasicInfoDto
        //                {
        //                    ActivityId = a.ActivityId,
        //                    Title = a.Title,
        //                    StartTime = a.StartTime,
        //                    EndTime = a.EndTime
        //                }).ToList(),
        //                ModificationSuggestions = modificationProposals // 🔥 nowe pole
        //            });
        //        }


        //    }

        //    // wypisz całość ładnie po zakończeniu
        //    Console.WriteLine("=== Aktywności nachodzące na sugerowane zakresy ===");
        //    Console.WriteLine(JsonSerializer.Serialize(overlapsPerSlot, new JsonSerializerOptions { WriteIndented = true }));

        //    return overlapsPerSlot;
        //}

        //// 3. "Zaskocz mnie" sugerowanie aktywnosci na podstawie losowych 100 użytkowników (którzy wyrazili zgode)
        //public async Task<IEnumerable<SuggestedTimelineActivityDto>> SuggestActivitiesFromCommunityAsync(int userId, ActivitySuggestionDto dto)
        //{
        //    var localZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

        //    Console.WriteLine("========== [DEBUG] COMMUNITY SUGGESTIONS ==========");

        //    // 1️⃣ Losowe 100 użytkowników z włączonym AllowDataStatistics
        //    var userPool = await _context.Users
        //        .Where(u => u.AllowDataStatistics && u.UserId != userId)
        //        .OrderBy(r => Guid.NewGuid())
        //        .Take(100)
        //        .Select(u => u.UserId)
        //        .ToListAsync();

        //    if (userPool.Count == 0)
        //    {
        //        Console.WriteLine("⚠️ Brak użytkowników z włączoną zgodą na statystyki.");
        //        return Enumerable.Empty<SuggestedTimelineActivityDto>();
        //    }

        //    Console.WriteLine($"Wybrano {userPool.Count} użytkowników do analizy.");

        //    // 2️⃣ Pobierz ich aktywności z ostatnich 2 miesięcy
        //    var referenceDate = DateTime.UtcNow.AddMonths(-2);

        //    var allActivities = await _context.TimelineActivities
        //        .Include(a => a.Category)
        //        .Where(a => userPool.Contains(a.OwnerId) && a.Start_time >= referenceDate)
        //        .ToListAsync();

        //    if (!allActivities.Any())
        //    {
        //        Console.WriteLine("⚠️ Brak danych aktywności do analizy.");
        //        return Enumerable.Empty<SuggestedTimelineActivityDto>();
        //    }

        //    // 3️⃣ Normalizacja: zamiana dat na lokalne i ekstrakcja cech
        //    var activityGroups = allActivities
        //        .Select(a =>
        //        {
        //            var local = TimeZoneInfo.ConvertTimeFromUtc(
        //                DateTime.SpecifyKind(a.Start_time, DateTimeKind.Utc), localZone);

        //            string timeSlot = local.Hour switch
        //            {
        //                >= 5 and < 12 => "morning",
        //                >= 12 and < 18 => "afternoon",
        //                _ => "evening"
        //            };

        //            return new
        //            {
        //                a.Title,
        //                a.CategoryId,
        //                CategoryName = a.Category?.Name ?? "Unknown",
        //                a.PlannedDurationMinutes,
        //                Day = local.DayOfWeek,
        //                TimeSlot = timeSlot
        //            };
        //        })
        //        .GroupBy(x => new { x.Title, x.CategoryId, x.CategoryName, x.TimeSlot, x.Day })
        //        .Select(g => new
        //        {
        //            g.Key.Title,
        //            g.Key.CategoryId,
        //            g.Key.CategoryName,
        //            g.Key.TimeSlot,
        //            g.Key.Day,
        //            Count = g.Count(),
        //            AvgDuration = g.Average(x => x.PlannedDurationMinutes)
        //        })
        //        .ToList();

        //    Console.WriteLine($"Zanalizowano {activityGroups.Count} różnych wzorców aktywności.");

        //    // 4️⃣ Oblicz zgodność z preferencjami użytkownika
        //    var suggestions = new List<SuggestedTimelineActivityDto>();

        //    foreach (var g in activityGroups)
        //    {
        //        double muDuration = 1.0;
        //        if (dto.PlannedDurationMinutes.HasValue)
        //        {
        //            var diff = Math.Abs(dto.PlannedDurationMinutes.Value - g.AvgDuration);
        //            muDuration = Math.Exp(-Math.Pow(diff / 60.0, 2)); // gaussian-like
        //        }

        //        double muDay = dto.PreferredDays != null && dto.PreferredDays.Count > 0
        //            ? (dto.PreferredDays.Contains(g.Day) ? 1.0 : 0.4)
        //            : 1.0;

        //        double muTime = 1.0;
        //        if (dto.PreferredStart.HasValue)
        //        {
        //            string preferredSlot = dto.PreferredStart.Value.Hours switch
        //            {
        //                >= 5 and < 12 => "morning",
        //                >= 12 and < 18 => "afternoon",
        //                _ => "evening"
        //            };
        //            muTime = (preferredSlot == g.TimeSlot) ? 1.0 : 0.5;
        //        }

        //        // liczba wystąpień jako waga popularności
        //        double popularity = Math.Min(1.0, g.Count / 25.0); // nasyca się przy 25+

        //        double score = 0.5 * muDuration + 0.2 * muDay + 0.2 * muTime + 0.1 * popularity;

        //        suggestions.Add(new SuggestedTimelineActivityDto
        //        {
        //            ActivityId = g.CategoryId ?? 0,
        //            Title = g.Title,
        //            CategoryName = g.CategoryName,
        //            SuggestedDurationMinutes = (int)Math.Round(g.AvgDuration),
        //            Score = Math.Round(score, 3)
        //        });
        //    }

        //    Console.WriteLine("========== [DEBUG] COMMUNITY END ==========");

        //    return suggestions
        //        .OrderByDescending(s => s.Score)
        //        .Take(5)
        //        .ToList();
        //}


    }
}
