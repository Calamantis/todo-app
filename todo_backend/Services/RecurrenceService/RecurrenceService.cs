using Microsoft.EntityFrameworkCore;

namespace todo_backend.Services.RecurrenceService
{
    public class RecurrenceService : IRecurrenceService
    {

        public IEnumerable<DateTime> GenerateOccurrences(DateTime start, string recurrenceRule, int daysAhead = 30)
        {
            var occurrences = new List<DateTime>();
            var ruleParts = recurrenceRule.Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Split('='))
                .ToDictionary(p => p[0], p => p[1]);

            int interval = ruleParts.ContainsKey("INTERVAL") ? int.Parse(ruleParts["INTERVAL"]) : 1;
            string type = ruleParts.ContainsKey("TYPE") ? ruleParts["TYPE"] : "DAY";
            var days = ruleParts.ContainsKey("DAYS") ? ruleParts["DAYS"].Split(',') : Array.Empty<string>();
            var times = ruleParts.ContainsKey("TIMES") ? ruleParts["TIMES"].Split(',') : new[] { "08:00" };

            var current = start;

            for (int i = 0; i < daysAhead; i++)
            {
                var date = current.AddDays(i);

                if (type == "DAY" && i % interval == 0)
                {
                    foreach (var time in times)
                        if (TimeSpan.TryParse(time, out var t))
                            occurrences.Add(date.Date + t);
                }
                else if (type == "WEEK" && days.Contains(date.DayOfWeek.ToString().Substring(0, 3).ToUpper()))
                {
                    foreach (var time in times)
                        if (TimeSpan.TryParse(time, out var t))
                            occurrences.Add(date.Date + t);
                }
            }

            return occurrences;
        }


    }
}
