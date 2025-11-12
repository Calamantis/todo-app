//using todo_backend.Dtos.Recurrence;
//using todo_backend.Models;

//namespace todo_backend.Services.RecurrenceService
//{
//    public interface IRecurrenceService
//    {
//        //IEnumerable<DateTime> GenerateOccurrences(DateTime start, string recurrenceRule,string recurrenceException, int daysAhead, DateTime? end);
//        //IEnumerable<DateTime> GenerateOccurrences(DateTime start, string recurrenceRule,string recurrenceException, DateTime from, DateTime to);


//        IEnumerable<DateTime> GenerateOccurrences(DateTime start,string recurrenceRule,IEnumerable<TimelineRecurrenceException>? exceptions, // wyjątki z bazy
//            int daysAhead = 365,
//            DateTime? end = null);


//        int? GetExceptionDuration(DateTime date, TimeSpan startTime);
//        //Task GenerateInitialInstancesAsync(TimelineActivity activity);
//        Task GenerateInstancesAsync(InstanceDto activity);
//    }
//}
