namespace todo_backend.Services.RecurrenceService
{
    public interface IRecurrenceService
    {
        IEnumerable<DateTime> GenerateOccurrences(DateTime start, string recurrenceRule,string recurrenceException, int daysAhead, DateTime? end);
        IEnumerable<DateTime> GenerateOccurrences(DateTime start, string recurrenceRule,string recurrenceException, DateTime from, DateTime to);
        int? GetExceptionDuration(DateTime date);
    }
}
