using System;
using System.Collections.Generic;
using System.Linq;

namespace todo_backend.Services.RecurrenceService
{
    public class RecurrenceService : IRecurrenceService
    {
        public IEnumerable<DateTime> GenerateOccurrences(DateTime start, string recurrenceRule, int daysAhead = 365)
        {
            var occurrences = new List<DateTime>();

            // 🔹 1. Parsowanie reguły
            var ruleParts = recurrenceRule
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Split('='))
                .ToDictionary(p => p[0].ToUpper(), p => p[1]);

            string type = ruleParts.ContainsKey("TYPE") ? ruleParts["TYPE"].ToUpper() : "DAY";
            int interval = ruleParts.ContainsKey("INTERVAL") ? int.Parse(ruleParts["INTERVAL"]) : 1;

            // pomocnicze dane
            var times = ruleParts.ContainsKey("TIMES")
                ? ruleParts["TIMES"].Split(',')
                : new[] { "08:00" };

            var days = ruleParts.ContainsKey("DAYS")
                ? ruleParts["DAYS"].Split(',')
                : Array.Empty<string>();

            // 🔸 Dla miesięcy i rocznych — osobne klucze
            var daysOfMonthRaw = ruleParts.ContainsKey("DAYS_OF_MONTH")
                ? ruleParts["DAYS_OF_MONTH"].Split(',')
                : Array.Empty<string>();

            var daysOfYearRaw = ruleParts.ContainsKey("DAYS_OF_YEAR")
                ? ruleParts["DAYS_OF_YEAR"].Split(',')
                : Array.Empty<string>();

            var current = start;

            // 🔹 2. Generowanie wystąpień
            for (int i = 0; i < daysAhead; i++)
            {
                var date = current.AddDays(i);

                switch (type)
                {
                    // ------------------ DZIENNE ------------------
                    case "DAY":
                        if (i % interval == 0)
                        {
                            foreach (var time in times)
                                if (TimeSpan.TryParse(time, out var t))
                                    occurrences.Add(date.Date + t);
                        }
                        break;

                    // ------------------ TYGODNIOWE ------------------
                    case "WEEK":
                        // Obsługa DAYS=TUE@14:00|17:00,WED@18:00
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

                    // ------------------ MIESIĘCZNE ------------------
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

                    // ------------------ ROCZNE ------------------
                    case "YEAR":
                        // DAYS_OF_YEAR=10-05@10:00,12-24@18:00
                        foreach (var yearDef in daysOfYearRaw)
                        {
                            var parts = yearDef.Split('@');
                            var dateToken = parts[0].Trim(); // np. 10-05
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

            // 🔹 3. Zwracanie wyników
            return occurrences.OrderBy(o => o);
        }
    }
}
