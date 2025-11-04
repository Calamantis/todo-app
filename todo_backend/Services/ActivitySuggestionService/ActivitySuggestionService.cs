using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
        // Opcja 1. sugerowanie na podstawie poprzednich aktywnosci uzytkownika
        public async Task<IEnumerable<SuggestedTimelineActivityDto>> SuggestActivitiesAsync(int userId,ActivitySuggestionDto dto)
        {
            var localZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

            // DEBUG: wejście użytkownika
            Console.WriteLine("========== [DEBUG] START SUGGESTION ==========");
            Console.WriteLine($"UserId: {userId}");
            Console.WriteLine($"Input DTO:");
            Console.WriteLine($"  PlannedDurationMinutes: {dto.PlannedDurationMinutes}");
            Console.WriteLine($"  PreferredStart: {dto.PreferredStart}");
            Console.WriteLine($"  PreferredEnd:   {dto.PreferredEnd}");
            Console.WriteLine($"  PreferredDays:  {(dto.PreferredDays != null ? string.Join(",", dto.PreferredDays) : "null")}");
            Console.WriteLine($"  CategoryId:     {dto.CategoryId}");
            Console.WriteLine("----------------------------------------------");

            var history = await _context.TimelineActivities
                .Include(a => a.Category)
                .Where(a => a.OwnerId == userId && a.Start_time > DateTime.UtcNow.AddMonths(-3))
                .ToListAsync();

            if (dto.CategoryId.HasValue)
                history = history.Where(a => a.CategoryId == dto.CategoryId.Value).ToList();

            var suggestions = new List<SuggestedTimelineActivityDto>();

            // Okno referencyjne do generowania slotów z rekurencji
            var occFromUtc = DateTime.UtcNow.AddMonths(-2);    // 2 m-ce wstecz
            var occToUtc = DateTime.UtcNow.AddDays(14);      // 2 tyg. do przodu

            foreach (var act in history)
            {
                // 1) μ_duration: ze średniej planowanych czasów dla tej nazwy
                var avgDuration = history
                    .Where(a => a.Title == act.Title && a.PlannedDurationMinutes > 0)
                    .DefaultIfEmpty()
                    .Average(a => a?.PlannedDurationMinutes ?? act.PlannedDurationMinutes);

                double muDuration = 1.0;
                if (dto.PlannedDurationMinutes.HasValue)
                {
                    var diff = Math.Abs(dto.PlannedDurationMinutes.Value - avgDuration);
                    if (diff <= 15)
                        muDuration = Math.Max(0.75, 1.0 - diff * 0.016);   // 1 → 0.75 w 15 min
                    else
                        muDuration = Math.Max(0.05, 0.75 - (diff - 15) * 0.05);
                    muDuration = Math.Round(muDuration, 3);
                }

                // 2) μ_time & μ_day: na podstawie WYSTĄPIEŃ Z REKURENCJI
                //    (jeśli brak rekurencji – bierz pojedynczy start jako slot)
                var candidateSlots = new List<(DayOfWeek day, TimeSpan time, DateTime local)>();

                if (act.Is_recurring && !string.IsNullOrWhiteSpace(act.Recurrence_rule))
                {
                    // Używamy Twojego serwisu, ale w wersji "z zakresem"
                    // Jeśli nie masz overloadu (start, rule, from, to) → zrób go zgodnie z poprzednimi wskazówkami
                    var occ = _recurrenceService.GenerateOccurrences(
                        act.Start_time, act.Recurrence_rule, occFromUtc, occToUtc);

                    // Zmapuj na lokalną strefę i zostaw tylko unikalne (dzień, godzina), żeby nie liczyć 100x tego samego
                    foreach (var o in occ.Take(200))
                    {
                        var loc = TimeZoneInfo.ConvertTimeFromUtc(
                            DateTime.SpecifyKind(o, DateTimeKind.Utc), localZone);
                        candidateSlots.Add((loc.DayOfWeek, loc.TimeOfDay, loc));
                    }

                    // DEBUG: pokaż kilka slotów
                    Console.WriteLine($"[DEBUG] Activity '{act.Title}' recurrence slots (sample):");
                    foreach (var s in candidateSlots.Take(5))
                        Console.WriteLine($"   - {s.local:yyyy-MM-dd HH:mm} ({s.day}, {s.time})");
                    if (candidateSlots.Count == 0)
                        Console.WriteLine("   - (no recurrence slots found in window)");
                }
                else
                {
                    // Jednorazowa → 1 slot z realnego startu
                    var loc = TimeZoneInfo.ConvertTimeFromUtc(
                        DateTime.SpecifyKind(act.Start_time, DateTimeKind.Utc), localZone);
                    candidateSlots.Add((loc.DayOfWeek, loc.TimeOfDay, loc));
                    Console.WriteLine($"[DEBUG] Activity '{act.Title}' single slot: {loc:yyyy-MM-dd HH:mm} ({loc.DayOfWeek}, {loc.TimeOfDay})");
                }

                // Jeżeli nie mamy żadnego slotu (np. reguła poza oknem) – pomiń tę aktywność
                if (candidateSlots.Count == 0)
                    continue;

                // Policz μ_time/μ_day dla KAŻDEGO slotu, weź najlepszy (max)
                double bestMuTime = 0.0;
                double bestMuDay = 0.0;
                (DayOfWeek day, TimeSpan time, DateTime local) bestSlot = default;

                foreach (var slot in candidateSlots)
                {
                    // μ_day
                    double muDay = 1.0;
                    if (dto.PreferredDays != null && dto.PreferredDays.Count > 0)
                        muDay = dto.PreferredDays.Contains(slot.day) ? 1.0 : 0.3;

                    // μ_time
                    double muTime = 1.0;
                    if (dto.PreferredStart.HasValue && dto.PreferredEnd.HasValue)
                    {
                        var startDiff = Math.Abs((slot.time - dto.PreferredStart.Value).TotalMinutes);
                        var endDiff = Math.Abs((slot.time - dto.PreferredEnd.Value).TotalMinutes);
                        var diff = Math.Min(startDiff, endDiff);

                        if (diff <= 15)
                            muTime = Math.Max(0.85, 1.0 - diff * 0.01);  // ±15 min ≈ 1 → 0.85
                        else
                            muTime = Math.Max(0.2, 0.85 - (diff - 15) * 0.03);
                    }

                    // DEBUG: μ dla tego slotu
                    Console.WriteLine($"   [slot] {slot.local:yyyy-MM-dd HH:mm} -> μ_time={muTime:F3}, μ_day={muDay:F3}");

                    // wybieramy slot z NAJLEPSZYM łącznym wkładem do finalnego wyniku (ważone)
                    var weighted = 0.25 * muTime + 0.15 * muDay; // wagi jak w głównym score
                    var bestWeighted = 0.25 * bestMuTime + 0.15 * bestMuDay;

                    if (weighted > bestWeighted)
                    {
                        bestMuTime = muTime;
                        bestMuDay = muDay;
                        bestSlot = slot;
                    }
                }

                // Finalny wynik
                double score = 0.6 * muDuration + 0.25 * bestMuTime + 0.15 * bestMuDay;

                Console.WriteLine($"[DEBUG] Activity: {act.Title}");
                Console.WriteLine($"  -> Avg Duration: {avgDuration} min");
                Console.WriteLine($"  -> BEST slot: {bestSlot.local:yyyy-MM-dd HH:mm} ({bestSlot.day}, {bestSlot.time})");
                Console.WriteLine($"  -> μ_duration={muDuration:F3}, μ_time(best)={bestMuTime:F3}, μ_day(best)={bestMuDay:F3}, SCORE={score:F3}");
                Console.WriteLine("----------------------------------------------");

                suggestions.Add(new SuggestedTimelineActivityDto
                {
                    ActivityId = act.ActivityId,
                    Title = act.Title,
                    CategoryName = act.Category?.Name,
                    SuggestedDurationMinutes = (int)Math.Round(avgDuration),
                    Score = Math.Round(score, 3),
                });
            }

            Console.WriteLine("========== [DEBUG] END ==========");

            return suggestions
                .OrderByDescending(s => s.Score)
                .Take(3)
                .ToList();
        }

        // Opcja 2.1. sugerowanie gdzie umiescic aktywność - bez modyfikacji osi czasu użytkownika
        public async Task<IEnumerable<DayFreeSummaryDto>> SuggestActivityPlacementAsync(int userId, ActivityPlacementSuggestionDto dto)
        {
            // 1) Aktywność i jej czas trwania
            var activity = await _context.TimelineActivities
                .FirstOrDefaultAsync(t => t.ActivityId == dto.ActivityId && t.OwnerId == userId);
            if (activity == null) return Enumerable.Empty<DayFreeSummaryDto>();

            var activityMinutes = activity.PlannedDurationMinutes; // np. 160
            var minRequired = activityMinutes + 20; // co najmniej 20 minut więcej

            // 2) Zakres analizy – bez konwersji
            var start = (dto.StartDate ?? DateTime.UtcNow);
            var end = (dto.EndDate ?? start.AddDays(14));
            if (end <= start) return Enumerable.Empty<DayFreeSummaryDto>();

            // 3) Dane osi czasu (zakładamy spójny czas z 'start'/'end')
            var userTimeline = await _timelineActivityService.GetTimelineForUserAsync(userId, start, end);

            // 4) Okno dzienne i ew. filtr dni tygodnia – bez konwersji
            var prefStart = dto.PreferredStart ?? TimeSpan.FromHours(6);   // 06:00
            var prefEnd = dto.PreferredEnd ?? TimeSpan.FromHours(22);  // 22:00
            var onlyDays = dto.PreferredDays != null ? new HashSet<DayOfWeek>(dto.PreferredDays) : null;

            // 5) Proste wydarzenia: od filtracji nulli do uporządkowanej listy
            var events = userTimeline
                .Select(e => new { Start = e.StartTime, End = e.EndTime })
                .Where(e => e.End > start && e.Start < end)
                .OrderBy(e => e.Start)
                .ToList();

            var result = new List<DayFreeSummaryDto>();

            // 6) Iteracja dzień po dniu (bez TZ – w tej samej skali co dane)
            for (var day = start.Date; day <= end.Date; day = day.AddDays(1))
            {
                if (onlyDays != null && !onlyDays.Contains(day.DayOfWeek)) continue;

                var windowStart = day + prefStart;
                var windowEnd = day + prefEnd;
                if (windowEnd <= windowStart) continue;

                // przycięcie do globalnego zakresu
                if (windowEnd <= start || windowStart >= end) continue;
                if (windowStart < start) windowStart = start;
                if (windowEnd > end) windowEnd = end;

                // Zdarzenia nachodzące okno
                var overlaps = events
                    .Select(e => new
                    {
                        S = e.Start < windowStart ? windowStart : e.Start,
                        E = e.End > windowEnd ? windowEnd : e.End
                    })
                    .Where(e => e.E > e.S)
                    .OrderBy(e => e.S)
                    .ToList();

                // Zbieramy luki
                var gaps = new List<(DateTime s, DateTime e)>();
                if (overlaps.Count == 0)
                {
                    gaps.Add((windowStart, windowEnd));
                }
                else
                {
                    if (overlaps[0].S > windowStart) gaps.Add((windowStart, overlaps[0].S));
                    for (int i = 0; i < overlaps.Count - 1; i++)
                    {
                        var gs = overlaps[i].E;
                        var ge = overlaps[i + 1].S;
                        if (ge > gs) gaps.Add(((DateTime s, DateTime e))(gs, ge));
                    }
                    if (overlaps[overlaps.Count - 1].E < windowEnd) gaps.Add(((DateTime s, DateTime e))(overlaps[overlaps.Count - 1].E, windowEnd));
                }

                // Dla każdej luki spełniającej warunek zwróć propozycję (w połowie luki)
                foreach (var (gs, ge) in gaps)
                {
                    var gapMinutes = (int)(ge - gs).TotalMinutes;
                    if (gapMinutes < minRequired) continue;

                    var slack = gapMinutes - activityMinutes;  // >= 20
                    var pad = slack / 2;                     // wyśrodkowanie
                    var sugStart = gs.AddMinutes(pad);
                    var sugEnd = sugStart.AddMinutes(activityMinutes);

                    result.Add(new DayFreeSummaryDto
                    {
                        DateLocal = day,                 // „dzień” tej luki
                        TotalFreeMinutes = gapMinutes,          // długość tej luki (jeśli chcesz, zmień nazwę pola)
                        SuggestedStart = sugStart,
                        SuggestedEnd = sugEnd
                    });
                }
            }

            return result;
        }

        // Opcja 2.2 sugerowanie gdzie umiescic aktywność - z modyfikację osi czasu użytkownika

        
    }
}
