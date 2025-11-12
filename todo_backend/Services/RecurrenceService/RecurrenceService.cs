//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using todo_backend.Models;
//using todo_backend.Data;
//using todo_backend.Dtos.TimelineRecurrenceExceptionDto;
//using todo_backend.Dtos.Recurrence;

//namespace todo_backend.Services.RecurrenceService
//{
//    public class RecurrenceService : IRecurrenceService
//    {
//        private readonly AppDbContext _context;

//        public RecurrenceService(AppDbContext context)
//        {
//            _context = context;
//        }

//        private readonly Dictionary<(DateTime date, TimeSpan startTime), int> _exceptionDurations = new();
//        public int? GetExceptionDuration(DateTime date, TimeSpan startTime) => _exceptionDurations.TryGetValue((date.Date, startTime), out var dur) ? dur : null;


//        public IEnumerable<DateTime> GenerateOccurrences(
//    DateTime start,
//    string recurrenceRule,
//    IEnumerable<TimelineRecurrenceException>? exceptions,
//    int daysAhead = 365,
//    DateTime? end = null)
//        {
//            var occurrences = new List<DateTime>();
//            _exceptionDurations.Clear();

//            // 🔹 1️⃣ Parsowanie reguły głównej
//            var ruleParts = recurrenceRule
//                .Split(';', StringSplitOptions.RemoveEmptyEntries)
//                .Select(p => p.Split('='))
//                .Where(p => p.Length == 2 && !string.IsNullOrWhiteSpace(p[0]))
//                .ToDictionary(p => p[0].Trim().ToUpper(), p => p[1].Trim());

//            string type = ruleParts.ContainsKey("TYPE") ? ruleParts["TYPE"].ToUpper() : "DAY";
//            int interval = ruleParts.ContainsKey("INTERVAL") ? int.Parse(ruleParts["INTERVAL"]) : 1;

//            var times = ruleParts.ContainsKey("TIMES")
//                ? ruleParts["TIMES"].Split(',', StringSplitOptions.RemoveEmptyEntries)
//                : new[] { "08:00" };

//            var days = ruleParts.ContainsKey("DAYS")
//                ? ruleParts["DAYS"].Split(',', StringSplitOptions.RemoveEmptyEntries)
//                : Array.Empty<string>();

//            var daysOfMonthRaw = ruleParts.ContainsKey("DAYS_OF_MONTH")
//                ? ruleParts["DAYS_OF_MONTH"].Split(',', StringSplitOptions.RemoveEmptyEntries)
//                : Array.Empty<string>();

//            var daysOfYearRaw = ruleParts.ContainsKey("DAYS_OF_YEAR")
//                ? ruleParts["DAYS_OF_YEAR"].Split(',', StringSplitOptions.RemoveEmptyEntries)
//                : Array.Empty<string>();


//            // 🔹 2️⃣ Przekształć wyjątki w słownik (dzień → lista wyjątków)
//            var exceptionMap = exceptions?
//                .GroupBy(e => e.ExceptionDate.Date)
//                .ToDictionary(g => g.Key, g => g.ToList())
//                ?? new Dictionary<DateTime, List<TimelineRecurrenceException>>();


//            // 🔹 3️⃣ Generowanie wystąpień
//            var current = start.Date;

//            for (int i = 0; i < daysAhead; i++)
//            {
//                var date = current.AddDays(i);
//                if (end != null && date.Date > end.Value.Date)
//                    break;


//                //🔸 4️⃣ Normalne generowanie wg reguły
//                switch (type)
//                {
//                    case "DAY":
//                        if (i % interval == 0)
//                        {
//                            foreach (var time in times)
//                                if (TimeSpan.TryParse(time, out var t))
//                                    occurrences.Add(date + t);
//                        }
//                        break;

//                    case "WEEK":
//                        foreach (var dayDef in days)
//                        {
//                            var parts = dayDef.Split('@', StringSplitOptions.RemoveEmptyEntries);
//                            var dayName = parts[0].Trim().ToUpper();
//                            var timeList = (parts.Length > 1 ? parts[1].Split('|') : times);

//                            if (date.DayOfWeek.ToString().Substring(0, 3).ToUpper() == dayName)
//                            {
//                                foreach (var time in timeList)
//                                    if (TimeSpan.TryParse(time, out var t))
//                                        occurrences.Add(date + t);
//                            }
//                        }
//                        break;

//                    case "MONTH":
//                        int dim = DateTime.DaysInMonth(date.Year, date.Month);
//                        foreach (var dayDef in daysOfMonthRaw)
//                        {
//                            var parts = dayDef.Split('@', StringSplitOptions.RemoveEmptyEntries);
//                            var dayToken = parts[0].Trim().ToUpper();
//                            var timeList = (parts.Length > 1 ? parts[1].Split('|') : times);

//                            int? targetDay = null;
//                            if (dayToken == "LAST")
//                                targetDay = dim;
//                            else if (int.TryParse(dayToken, out int parsed))
//                                targetDay = parsed <= dim ? parsed : null;

//                            if (targetDay.HasValue && date.Day == targetDay.Value)
//                            {
//                                foreach (var time in timeList)
//                                    if (TimeSpan.TryParse(time, out var t))
//                                        occurrences.Add(date + t);
//                            }
//                        }
//                        break;

//                    case "YEAR":
//                        foreach (var yearDef in daysOfYearRaw)
//                        {
//                            var parts = yearDef.Split('@', StringSplitOptions.RemoveEmptyEntries);
//                            var dateToken = parts[0].Trim();
//                            var timeList = (parts.Length > 1 ? parts[1].Split('|') : times);

//                            if (DateTime.TryParseExact(dateToken, "MM-dd", null,
//                                System.Globalization.DateTimeStyles.None, out var target))
//                            {
//                                if (date.Month == target.Month && date.Day == target.Day)
//                                {
//                                    foreach (var time in timeList)
//                                        if (TimeSpan.TryParse(time, out var t))
//                                            occurrences.Add(date + t);
//                                }
//                            }
//                        }
//                        break;
//                }
//            }

//            foreach (var oc in occurrences)
//            {
//                Console.WriteLine(oc);
//            }

//            return occurrences.OrderBy(o => o);
//        }




//        public async Task GenerateInstancesAsync(InstanceDto activity)
//        {
//            if (!activity.Is_recurring || string.IsNullOrWhiteSpace(activity.Recurrence_rule))
//                return;

//            Console.WriteLine("\n\n\n[JESTEM W GENERATEINSTANCESASYNC DLA ID ] " + activity.ActivityId + " [////////////////]" + " \n\n\n");


//            var start = activity.Start_time.Date;
//            var endLimit = activity.End_time ?? DateTime.UtcNow.AddDays(30);

//            // 🔹 Pobierz wyjątki dla tej aktywności
//            var exceptions = await _context.TimelineRecurrenceExceptions
//                .Where(e => e.ActivityId == activity.ActivityId)
//                .ToListAsync();

//            // 🔹 Wygeneruj wszystkie wystąpienia w zakresie
//            var occurrences = GenerateOccurrences(
//                activity.Start_time,
//                activity.Recurrence_rule,
//                exceptions,
//                (int)(endLimit - start).TotalDays,
//                end: endLimit);

//            if (!occurrences.Any())
//                return;

//            // 🔹 Pobierz już istniejące wystąpienia
//            var existingInstances = await _context.TimelineRecurrenceInstances
//                .Where(i => i.ActivityId == activity.ActivityId)
//                .Select(i => new { i.OccurrenceDate, i.StartTime })
//                .ToListAsync();

//            var newInstances = new List<TimelineRecurrenceInstance>();

//            foreach (var occ in occurrences)
//            {
//                var date = occ.Date;
//                var time = occ.TimeOfDay;

//                // wyjątki dla tego dnia
//                var dayExceptions = exceptions.Where(e => e.ExceptionDate.Date == date).ToList();

//                // 🔸 1️⃣ SKIP → usuń wszystkie wystąpienia z tej daty i pomiń generowanie
//                if (dayExceptions.Any(e => e.Mode == RecurrenceExceptionMode.SkipExisting))
//                {
//                    var toRemove = await _context.TimelineRecurrenceInstances
//                        .Where(i => i.ActivityId == activity.ActivityId && i.OccurrenceDate == date)
//                        .ToListAsync();

//                    if (toRemove.Any())
//                        _context.TimelineRecurrenceInstances.RemoveRange(toRemove);

//                    continue;
//                }

//                // 🔸 2️⃣ REPLACE → usuń stare i dodaj tylko nowy wpis z nowym startem/czasem trwania
//                var replaceEx = dayExceptions.FirstOrDefault(e => e.Mode == RecurrenceExceptionMode.ReplaceExisting);
//                if (replaceEx != null)
//                {
//                    // usuń wszystkie wystąpienia dla tej daty
//                    var toRemove = await _context.TimelineRecurrenceInstances
//                        .Where(i => i.ActivityId == activity.ActivityId && i.OccurrenceDate == date)
//                        .ToListAsync();

//                    if (toRemove.Any())
//                        _context.TimelineRecurrenceInstances.RemoveRange(toRemove);

//                    var duration = replaceEx.NewDurationMinutes ?? activity.PlannedDurationMinutes;
//                    var startTime = replaceEx.NewStartTime ?? time;

//                    newInstances.Add(new TimelineRecurrenceInstance
//                    {
//                        ActivityId = activity.ActivityId,
//                        OccurrenceDate = date,
//                        StartTime = startTime,
//                        EndTime = startTime.Add(TimeSpan.FromMinutes(duration)),
//                        DurationMinutes = duration
//                    });

//                    continue;
//                }


//                var additionalExList = dayExceptions
//                .Where(e => e.Mode == RecurrenceExceptionMode.AddAdditional)
//                .ToList();

//                if (additionalExList.Any())
//                {
//                    foreach (var addEx in additionalExList)
//                    {
//                        newInstances.Add(new TimelineRecurrenceInstance
//                        {
//                            ActivityId = activity.ActivityId,
//                            OccurrenceDate = date,
//                            StartTime = addEx.NewStartTime ?? time,
//                            DurationMinutes = addEx.NewDurationMinutes ?? activity.PlannedDurationMinutes
//                        });
//                    }
//                    // kontynuujemy dalej, bo mogą istnieć też normalne wystąpienia
//                }

//                // 🔸 4️⃣ NORMALNE WYSTĄPIENIE → dodaj, jeśli nie ma duplikatu (data + godzina)
//                bool exists = existingInstances.Any(i => i.OccurrenceDate == date && i.StartTime == time);
//                if (exists)
//                    continue;

//                var baseDuration = GetExceptionDuration(date, time) ?? activity.PlannedDurationMinutes;

//                newInstances.Add(new TimelineRecurrenceInstance
//                {
//                    ActivityId = activity.ActivityId,
//                    OccurrenceDate = date,
//                    StartTime = time,
//                    EndTime = time.Add(TimeSpan.FromMinutes(baseDuration)),
//                    DurationMinutes = baseDuration
//                });
//            }

//            // 🔹 Zapisz zmiany do bazy
//            if (newInstances.Any())
//            {
//                _context.TimelineRecurrenceInstances.AddRange(newInstances);
//                await _context.SaveChangesAsync();
//            }
//        }



//    }
//}
