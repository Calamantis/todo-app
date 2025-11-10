using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace todo_backend.Services.RecurrenceService
{
    public class RecurrenceService : IRecurrenceService
    {
        //public IEnumerable<DateTime> GenerateOccurrences(DateTime start, string recurrenceRule, string recurrenceException, int daysAhead = 365)
        //{

        //    var occurrences = new List<DateTime>();

        //    // 🔹 1. Parsowanie reguły głównej
        //    var ruleParts = recurrenceRule
        //        .Split(';', StringSplitOptions.RemoveEmptyEntries)
        //        .Select(p => p.Split('='))
        //        .Where(p => p.Length == 2 && !string.IsNullOrWhiteSpace(p[0]))
        //        .ToDictionary(p => p[0].Trim().ToUpper(), p => p[1].Trim());

        //    string type = ruleParts.ContainsKey("TYPE") ? ruleParts["TYPE"].ToUpper() : "DAY";
        //    int interval = ruleParts.ContainsKey("INTERVAL") ? int.Parse(ruleParts["INTERVAL"]) : 1;

        //    var times = ruleParts.ContainsKey("TIMES")
        //        ? ruleParts["TIMES"].Split(',')
        //        : new[] { "08:00" };

        //    var days = ruleParts.ContainsKey("DAYS")
        //        ? ruleParts["DAYS"].Split(',')
        //        : Array.Empty<string>();

        //    var daysOfMonthRaw = ruleParts.ContainsKey("DAYS_OF_MONTH")
        //        ? ruleParts["DAYS_OF_MONTH"].Split(',')
        //        : Array.Empty<string>();

        //    var daysOfYearRaw = ruleParts.ContainsKey("DAYS_OF_YEAR")
        //        ? ruleParts["DAYS_OF_YEAR"].Split(',')
        //        : Array.Empty<string>();

        //    // 🔹 2. Parsowanie wyjątków (jeśli istnieją)
        //    var exceptions = new Dictionary<DateTime, string[]?>();
        //    if (!string.IsNullOrWhiteSpace(recurrenceException))
        //    {
        //        var exLines = recurrenceException
        //            .Split(';', StringSplitOptions.RemoveEmptyEntries);

        //        foreach (var ex in exLines)
        //        {
        //            var parts = ex.Split('@', StringSplitOptions.RemoveEmptyEntries);
        //            if (parts.Length == 0) continue;

        //            // Data: np. 20251106
        //            if (DateTime.TryParseExact(parts[0], "yyyyMMdd", null,
        //                System.Globalization.DateTimeStyles.None, out var exDate))
        //            {
        //                if (parts.Length == 1 || parts[1].Trim().ToUpper() == "SKIP")
        //                {
        //                    exceptions[exDate.Date] = null; // SKIP day
        //                }
        //                else if (parts[1].StartsWith("TIMES=", StringComparison.OrdinalIgnoreCase))
        //                {
        //                    var timesPart = parts[1].Substring("TIMES=".Length);
        //                    var exTimes = timesPart.Split(',', StringSplitOptions.RemoveEmptyEntries);
        //                    exceptions[exDate.Date] = exTimes;
        //                }
        //            }
        //        }
        //    }

        //    // 🔹 3. Generowanie wystąpień
        //    var current = start;

        //    for (int i = 0; i < daysAhead; i++)
        //    {
        //        var date = current.AddDays(i);
        //        var dateKey = date.Date;

        //        // 🔸 Sprawdź, czy istnieje wyjątek dla tej daty
        //        if (exceptions.ContainsKey(dateKey))
        //        {
        //            var exTimes = exceptions[dateKey];

        //            // SKIP – pomiń ten dzień całkowicie
        //            if (exTimes == null)
        //                continue;

        //            // Zastąp godziny z wyjątku
        //            foreach (var time in exTimes)
        //                if (TimeSpan.TryParse(time, out var t))
        //                    occurrences.Add(date.Date + t);

        //            // wyjątek zawsze nadpisuje, więc nie generujemy dalej z reguły
        //            continue;
        //        }

        //        // 🔸 Brak wyjątku – generuj normalnie wg reguły
        //        switch (type)
        //        {
        //            case "DAY":
        //                if (i % interval == 0)
        //                {
        //                    foreach (var time in times)
        //                        if (TimeSpan.TryParse(time, out var t))
        //                            occurrences.Add(date.Date + t);
        //                }
        //                break;

        //            case "WEEK":
        //                foreach (var dayDef in days)
        //                {
        //                    var parts = dayDef.Split('@');
        //                    var dayName = parts[0].Trim().ToUpper();
        //                    var timeList = (parts.Length > 1 ? parts[1].Split('|') : times);

        //                    if (date.DayOfWeek.ToString().Substring(0, 3).ToUpper() == dayName)
        //                    {
        //                        foreach (var time in timeList)
        //                            if (TimeSpan.TryParse(time, out var t))
        //                                occurrences.Add(date.Date + t);
        //                    }
        //                }
        //                break;

        //            case "MONTH":
        //                int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
        //                foreach (var dayDef in daysOfMonthRaw)
        //                {
        //                    var parts = dayDef.Split('@');
        //                    var dayToken = parts[0].Trim().ToUpper();
        //                    var timeList = (parts.Length > 1 ? parts[1].Split('|') : times);

        //                    int? targetDay = null;
        //                    if (dayToken == "LAST")
        //                        targetDay = daysInMonth;
        //                    else if (int.TryParse(dayToken, out int parsed))
        //                        targetDay = parsed <= daysInMonth ? parsed : null;

        //                    if (targetDay.HasValue && date.Day == targetDay.Value)
        //                    {
        //                        foreach (var time in timeList)
        //                            if (TimeSpan.TryParse(time, out var t))
        //                                occurrences.Add(date.Date + t);
        //                    }
        //                }
        //                break;

        //            case "YEAR":
        //                foreach (var yearDef in daysOfYearRaw)
        //                {
        //                    var parts = yearDef.Split('@');
        //                    var dateToken = parts[0].Trim();
        //                    var timeList = (parts.Length > 1 ? parts[1].Split('|') : times);

        //                    if (DateTime.TryParseExact(dateToken, "MM-dd", null,
        //                        System.Globalization.DateTimeStyles.None, out var target))
        //                    {
        //                        if (date.Month == target.Month && date.Day == target.Day)
        //                        {
        //                            foreach (var time in timeList)
        //                                if (TimeSpan.TryParse(time, out var t))
        //                                    occurrences.Add(date.Date + t);
        //                        }
        //                    }
        //                }
        //                break;
        //        }
        //    }

        //    return occurrences.OrderBy(o => o);

        //}



        public IEnumerable<DateTime> GenerateOccurrences(
    DateTime start,
    string recurrenceRule,
    string recurrenceException,
    int daysAhead = 365,
    DateTime? end = null)
        {
            var occurrences = new List<DateTime>();

            // Słownik: data -> czas trwania z wyjątku (minuty)
            _exceptionDurations.Clear();

            // 🔹 1. Parsowanie reguły głównej
            var ruleParts = recurrenceRule
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Split('='))
                .Where(p => p.Length == 2 && !string.IsNullOrWhiteSpace(p[0]))
                .ToDictionary(p => p[0].Trim().ToUpper(), p => p[1].Trim());

            string type = ruleParts.ContainsKey("TYPE") ? ruleParts["TYPE"].ToUpper() : "DAY";
            int interval = ruleParts.ContainsKey("INTERVAL") ? int.Parse(ruleParts["INTERVAL"]) : 1;

            var times = ruleParts.ContainsKey("TIMES")
                ? ruleParts["TIMES"].Split(',')
                : new[] { "08:00" };

            var days = ruleParts.ContainsKey("DAYS")
                ? ruleParts["DAYS"].Split(',')
                : Array.Empty<string>();

            var daysOfMonthRaw = ruleParts.ContainsKey("DAYS_OF_MONTH")
                ? ruleParts["DAYS_OF_MONTH"].Split(',')
                : Array.Empty<string>();

            var daysOfYearRaw = ruleParts.ContainsKey("DAYS_OF_YEAR")
                ? ruleParts["DAYS_OF_YEAR"].Split(',')
                : Array.Empty<string>();

            // 🔹 2. Parsowanie wyjątków (TIMES i DUR)
            var exceptions = new Dictionary<DateTime, string[]?>();
            if (!string.IsNullOrWhiteSpace(recurrenceException))
            {
                var exLines = recurrenceException.Split(';', StringSplitOptions.RemoveEmptyEntries);

                foreach (var ex in exLines)
                {
                    var parts = ex.Split('@', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0) continue;

                    // Data np. 20251110
                    if (DateTime.TryParseExact(parts[0], "yyyyMMdd", null,
                        System.Globalization.DateTimeStyles.None, out var exDate))
                    {
                        if (parts.Length == 1 || parts[1].Trim().ToUpper() == "SKIP")
                        {
                            exceptions[exDate.Date] = null;
                        }
                        else
                        {
                            // Możliwe: TIMES=07:00|DUR=480 albo TIMES=07:00,09:00|DUR=60
                            string[]? timesArr = null;
                            int? duration = null;

                            var tokens = parts[1].Split('|', StringSplitOptions.RemoveEmptyEntries);
                            foreach (var token in tokens)
                            {
                                if (token.StartsWith("TIMES=", StringComparison.OrdinalIgnoreCase))
                                {
                                    var timesStr = token.Substring("TIMES=".Length);
                                    timesArr = timesStr.Split(',', StringSplitOptions.RemoveEmptyEntries);
                                }
                                else if (token.StartsWith("DUR=", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (int.TryParse(token.Substring("DUR=".Length), out var parsedDur))
                                        duration = parsedDur;
                                }
                            }

                            if (duration.HasValue)
                                _exceptionDurations[exDate.Date] = duration.Value;

                            exceptions[exDate.Date] = timesArr ?? Array.Empty<string>();
                        }
                    }
                }
            }

            // 🔹 3. Generowanie wystąpień
            var current = start;

            for (int i = 0; i < daysAhead; i++)
            {
                var date = current.AddDays(i);

                if (end != null && date.Date > end.Value.Date)
                    break; // zakończ generowanie

                var dateKey = date.Date;

                if (exceptions.ContainsKey(dateKey))
                {
                    var exTimes = exceptions[dateKey];
                    if (exTimes == null) continue;

                    foreach (var time in exTimes)
                    {
                        if (TimeSpan.TryParse(time, out var t))
                            occurrences.Add(date.Date + t);
                    }
                    continue;
                }

                // 🔸 Brak wyjątku — generuj normalnie
                switch (type)
                {
                    case "DAY":
                        if (i % interval == 0)
                        {
                            foreach (var time in times)
                                if (TimeSpan.TryParse(time, out var t))
                                    occurrences.Add(date.Date + t);
                        }
                        break;

                    case "WEEK":
                        foreach (var dayDef in days)
                        {
                            var parts = dayDef.Split('@');
                            var dayName = parts[0].Trim().ToUpper();
                            var timeList = (parts.Length > 1 ? parts[1].Split('|') : times);

                            if (date.DayOfWeek.ToString().Substring(0, 3).ToUpper() == dayName)
                            {
                                foreach (var time in timeList)
                                    if (TimeSpan.TryParse(time, out var t))
                                        occurrences.Add(date.Date + t);
                            }
                        }
                        break;

                    case "MONTH":
                        int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
                        foreach (var dayDef in daysOfMonthRaw)
                        {
                            var parts = dayDef.Split('@');
                            var dayToken = parts[0].Trim().ToUpper();
                            var timeList = (parts.Length > 1 ? parts[1].Split('|') : times);

                            int? targetDay = null;
                            if (dayToken == "LAST")
                                targetDay = daysInMonth;
                            else if (int.TryParse(dayToken, out int parsed))
                                targetDay = parsed <= daysInMonth ? parsed : null;

                            if (targetDay.HasValue && date.Day == targetDay.Value)
                            {
                                foreach (var time in timeList)
                                    if (TimeSpan.TryParse(time, out var t))
                                        occurrences.Add(date.Date + t);
                            }
                        }
                        break;

                    case "YEAR":
                        foreach (var yearDef in daysOfYearRaw)
                        {
                            var parts = yearDef.Split('@');
                            var dateToken = parts[0].Trim();
                            var timeList = (parts.Length > 1 ? parts[1].Split('|') : times);

                            if (DateTime.TryParseExact(dateToken, "MM-dd", null,
                                System.Globalization.DateTimeStyles.None, out var target))
                            {
                                if (date.Month == target.Month && date.Day == target.Day)
                                {
                                    foreach (var time in timeList)
                                        if (TimeSpan.TryParse(time, out var t))
                                            occurrences.Add(date.Date + t);
                                }
                            }
                        }
                        break;
                }
            }

            return occurrences.OrderBy(o => o);
        }



        private readonly Dictionary<DateTime, int> _exceptionDurations = new();
        public int? GetExceptionDuration(DateTime date) => _exceptionDurations.TryGetValue(date.Date, out var dur) ? dur : null;


        public IEnumerable<DateTime> GenerateOccurrences(
    DateTime start,
    string recurrenceRule,
    string recurrenceException,
    DateTime from,
    DateTime to)
        {
            // ✅ Jeśli 'to' < 'from' – zwróć pustą listę
            if (to <= from)
                return Enumerable.Empty<DateTime>();

            // 🔹 Ustal zakres dniAhead na podstawie różnicy
            int daysAhead = (int)Math.Ceiling((to - from).TotalDays);
            if (daysAhead <= 0) daysAhead = 1;

            // 🔹 Wygeneruj pełną listę wystąpień (używa Twojej głównej metody)
            var allOccurrences = GenerateOccurrences(start, recurrenceRule, recurrenceException, daysAhead);

            // 🔹 Filtruj wystąpienia w podanym zakresie
            var filtered = allOccurrences
                .Where(o => o >= from && o <= to)
                .OrderBy(o => o)
                .ToList();

            return filtered;
        }






    }
}
