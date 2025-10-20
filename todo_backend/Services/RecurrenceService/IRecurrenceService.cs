namespace todo_backend.Services.RecurrenceService
{
    public interface IRecurrenceService
    {
        IEnumerable<DateTime> GenerateOccurrences(DateTime start, string recurrenceRule, int daysAhead);
    }
}
