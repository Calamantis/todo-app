using Microsoft.EntityFrameworkCore;
using System.Linq;
using todo_backend.Data;
using todo_backend.Dtos.ActivitySuggestionDto;
using todo_backend.Services.RecurrenceService;

namespace todo_backend.Services.ActivitySuggestionService
{
    public class ActivitySuggestionService : IActivitySuggestionService
    {
        private readonly AppDbContext _context;
        private readonly IRecurrenceService _recurrenceService;

        public ActivitySuggestionService(AppDbContext context, IRecurrenceService recurrenceService)
        {
            _context = context;
            _recurrenceService = recurrenceService;
        }

        //public async Task<IEnumerable<SuggestedTimelineActivityDto>> SuggestActivitiesAsync(int userId, ActivitySuggestionDto dto)
        //{
        //    var history = await _context.TimelineActivities
        //        .Include(a => a.Category)
        //        .Where(a => a.OwnerId == userId && a.Start_time > DateTime.UtcNow.AddMonths(-3))
        //        .ToListAsync();

        //    if (dto.CategoryId.HasValue)
        //        history = history.Where(a => a.CategoryId == dto.CategoryId.Value).ToList();

        //    var suggestions = new List<SuggestedTimelineActivityDto>();

        //    foreach (var act in history)
        //    {
        //        var avgDuration = history
        //            .Where(a => a.Title == act.Title && a.PlannedDurationMinutes > 0)
        //            .Average(a => a.PlannedDurationMinutes);

        //        // --- Fuzzy dopasowanie czasu trwania ---
        //        double μ_duration = 1.0;
        //        if (dto.PlannedDurationMinutes.HasValue)
        //        {
        //            var diff = Math.Abs(dto.PlannedDurationMinutes.Value - avgDuration);
        //            μ_duration = Math.Max(0, 1 - diff / (double)(dto.PlannedDurationMinutes.Value + 1));
        //        }

        //        // --- Fuzzy dopasowanie godziny ---
        //        double μ_time = 1.0;
        //        if (dto.PreferredStart.HasValue && dto.PreferredEnd.HasValue)
        //        {
        //            var actStart = act.Start_time.ToLocalTime().TimeOfDay;
        //            if (actStart < dto.PreferredStart)
        //            {
        //                var diff = (dto.PreferredStart.Value - actStart).TotalMinutes;

        //                if (diff <= 15)
        //                    μ_time = Math.Max(0.85, 1.0 - diff * 0.01); // spadek łagodny
        //                else
        //                    μ_time = Math.Max(0.0, 0.85 - (diff - 15) * 0.03); // szybszy spadek
        //            }
        //            else if (actStart > dto.PreferredEnd)
        //            {
        //                var diff = (actStart - dto.PreferredEnd.Value).TotalMinutes;

        //                if (diff <= 15)
        //                    μ_time = Math.Max(0.85, 1.0 - diff * 0.01);
        //                else
        //                    μ_time = Math.Max(0.0, 0.85 - (diff - 15) * 0.03);
        //            }
        //            else
        //            {
        //                μ_time = 1.0; // idealnie w widełkach
        //            }
        //        }


        //        // --- Fuzzy dopasowanie dnia tygodnia ---
        //        double μ_day = 1.0;
        //        if (dto.PreferredDays != null && dto.PreferredDays.Count > 0)
        //        {
        //            var actDay = act.Start_time.ToLocalTime().DayOfWeek;
        //            μ_day = dto.PreferredDays.Contains(act.Start_time.DayOfWeek) ? 1.0 : 0.3;
        //        }


        //        // --- Łączny wynik fuzzy ---
        //        double score = 0.6 * μ_duration + 0.25 * μ_time + 0.15 * μ_day;


        //        suggestions.Add(new SuggestedTimelineActivityDto
        //        {
        //            ActivityId = act.ActivityId,
        //            Title = act.Title,
        //            CategoryName = act.Category?.Name,
        //            SuggestedDurationMinutes = (int)Math.Round(avgDuration),
        //            Score = Math.Round(score, 3)
        //        });
        //    }

        //    return suggestions
        //        .OrderByDescending(s => s.Score)
        //        .Take(3)
        //        .ToList();
        //}


        //public async Task<IEnumerable<SuggestedTimelineActivityDto>> SuggestActivitiesAsync(int userId, ActivitySuggestionDto dto)
        //{
        //    var history = await _context.TimelineActivities
        //        .Include(a => a.Category)
        //        .Where(a => a.OwnerId == userId && a.Start_time > DateTime.UtcNow.AddMonths(-3))
        //        .ToListAsync();

        //    if (dto.CategoryId.HasValue)
        //        history = history.Where(a => a.CategoryId == dto.CategoryId.Value).ToList();

        //    var suggestions = new List<SuggestedTimelineActivityDto>();

        //    foreach (var act in history)
        //    {
        //        //streafa czasowa
        //        var localZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        //        var actLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(act.Start_time, DateTimeKind.Utc), localZone);

        //        Console.WriteLine("========== [DEBUG] START SUGGESTION ==========");
        //        Console.WriteLine($"UserId: {userId}");
        //        Console.WriteLine($"Input DTO:");
        //        Console.WriteLine($"  PlannedDurationMinutes: {dto.PlannedDurationMinutes}");
        //        Console.WriteLine($"  PreferredStart: {dto.PreferredStart}");
        //        Console.WriteLine($"  PreferredEnd:   {dto.PreferredEnd}");
        //        Console.WriteLine($"  PreferredDays:  {(dto.PreferredDays != null ? string.Join(",", dto.PreferredDays) : "null")}");
        //        Console.WriteLine($"  CategoryId:     {dto.CategoryId}");
        //        Console.WriteLine("----------------------------------------------");

        //        var avgDuration = history
        //            .Where(a => a.Title == act.Title && a.PlannedDurationMinutes > 0)
        //            .Average(a => a.PlannedDurationMinutes);

        //        // --- 1️⃣ Fuzzy dopasowanie czasu trwania ---
        //        double μ_duration = 1.0;
        //        if (dto.PlannedDurationMinutes.HasValue)
        //        {
        //            var diff = Math.Abs(dto.PlannedDurationMinutes.Value - avgDuration);

        //            if (diff == 0)
        //                μ_duration = 1.0;
        //            else if (diff <= 15)
        //                μ_duration = Math.Max(0.75, 1 - diff * 0.015);  // do 15 min spada delikatnie
        //            else
        //                μ_duration = Math.Max(0.05, 0.75 - (diff - 15) * 0.05); // po 15 min spada mocniej
        //        }

        //        // --- 2️⃣ Fuzzy dopasowanie godzin (z buforem ±15 min) ---
        //        double μ_time = 1.0;
        //        if (dto.PreferredStart.HasValue && dto.PreferredEnd.HasValue)
        //        {
        //            var actStart = actLocal.TimeOfDay;
        //            var prefStart = dto.PreferredStart.Value;
        //            var prefEnd = dto.PreferredEnd.Value;

        //            if (actStart < prefStart || actStart > prefEnd)
        //            {
        //                var diffStart = Math.Abs((actStart - prefStart).TotalMinutes);
        //                var diffEnd = Math.Abs((actStart - prefEnd).TotalMinutes);
        //                var minDiff = Math.Min(diffStart, diffEnd);

        //                if (minDiff <= 15)
        //                    μ_time = 1.0 - (minDiff / 15.0) * 0.01; // 15 min bufor
        //                else if (minDiff <= 60)
        //                    μ_time = Math.Max(0.85, 1 - (minDiff - 15) * 0.03); // łagodne obniżenie do 0.85
        //                else
        //                    μ_time = Math.Max(0.2, 0.85 - (minDiff - 60) * 0.05); // szybki spadek
        //            }
        //        }

        //        var actDay = actLocal.DayOfWeek;
        //        // --- 3️⃣ Fuzzy dopasowanie dnia tygodnia ---
        //        double μ_day = 1.0;
        //        if (dto.PreferredDays != null && dto.PreferredDays.Count > 0)
        //        {

        //            μ_day = (dto.PreferredDays != null && dto.PreferredDays.Contains(actDay)) ? 1.0 : 0.3;
        //        }

        //        // --- 4️⃣ Łączny wynik fuzzy (waga: czas > godzina > dzień) ---
        //        double score = 0.6 * μ_duration + 0.25 * μ_time + 0.15 * μ_day;
        //        Console.WriteLine($"[DEBUG] {act.Title}: μ_duration={μ_duration:F3}, μ_time={μ_time:F3}, μ_day={μ_day:F3}, score={score:F3}");

        //        // 🔹 Debug — pokaz dokładne porównania
        //        Console.WriteLine($"[DEBUG] Activity: {act.Title}");
        //        Console.WriteLine($"  -> Act StartTime (UTC): {act.Start_time}");
        //        Console.WriteLine($"  -> Act LocalTime: {actLocal} ({actDay})");
        //        Console.WriteLine($"  -> Avg Duration: {avgDuration} min");
        //        Console.WriteLine($"  -> μ_duration={μ_duration:F3}, μ_time={μ_time:F3}, μ_day={μ_day:F3}, SCORE={score:F3}");
        //        Console.WriteLine("----------------------------------------------");

        //        Console.WriteLine("========== [DEBUG] END ==========");
        //        suggestions.Add(new SuggestedTimelineActivityDto
        //        {
        //            ActivityId = act.ActivityId,
        //            Title = act.Title,
        //            CategoryName = act.Category?.Name,
        //            SuggestedDurationMinutes = (int)Math.Round(avgDuration),
        //            Score = Math.Round(score, 3)
        //        });
        //    }

        //    return suggestions
        //        .OrderByDescending(s => s.Score)
        //        .Take(3)
        //        .ToList();
        //}


        public async Task<IEnumerable<SuggestedTimelineActivityDto>> SuggestActivitiesAsync(
    int userId,
    ActivitySuggestionDto dto)
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



    }
}
