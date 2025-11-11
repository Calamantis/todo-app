using Microsoft.EntityFrameworkCore;

using todo_backend.Data;
using todo_backend.Dtos.ActivitySuggestionDto;
using todo_backend.Dtos.Friendship;
using todo_backend.Dtos.Recurrence;
using todo_backend.Dtos.TimelineActivity;
using todo_backend.Models;
using todo_backend.Services.RecurrenceService;

namespace todo_backend.Services.TimelineActivityService
{
    public class TimelineActivityService : ITimelineActivityService
    {
        private readonly AppDbContext _context;
        private readonly IRecurrenceService _recurrenceService;

        public TimelineActivityService(AppDbContext context, IRecurrenceService recurrenceService) 
        { 
            _context = context;
            _recurrenceService = recurrenceService;
        }

        //GET przeglądanie (swoich) aktywnosci
        public async Task<IEnumerable<FullTimelineActivityDto?>> GetTimelineActivitiesAsync(int id)
        {
            return await _context.TimelineActivities
                        .Where(t => t.OwnerId == id)
                        .Select(t => new FullTimelineActivityDto
                        {
                            ActivityId = t.ActivityId,
                            Title = t.Title,
                            Description = t.Description,
                            StartTime = t.Start_time,
                            EndTime = t.End_time,
                            IsRecurring = t.Is_recurring,
                            RecurrenceRule = t.Recurrence_rule,
                            PlannedDurationMinutes = t.PlannedDurationMinutes,
                            CategoryName = t.Category != null ? t.Category.Name : null,
                            JoinCode = t.JoinCode
                        })
                        .ToListAsync();
        }

        //GET po id
        public async Task<FullTimelineActivityDto?> GetTimelineActivityByIdAsync(int activityId, int currentUserId)
        {
            var activity = await _context.TimelineActivities
                            .Include(t => t.Category)
                            .FirstOrDefaultAsync(t => t.ActivityId == activityId && t.OwnerId == currentUserId);

            if (activity == null) return null;

            return new FullTimelineActivityDto
            {
                ActivityId = activity.ActivityId,
                Title = activity.Title,
                Description = activity.Description,
                StartTime = activity.Start_time,
                EndTime = activity.End_time,
                IsRecurring = activity.Is_recurring,
                RecurrenceRule = activity.Recurrence_rule,
                PlannedDurationMinutes = activity.PlannedDurationMinutes,
                CategoryName = activity.Category != null ? activity.Category.Name : null,
                JoinCode = activity.JoinCode
            };

        }

        //POST stworzenie aktywnosci
        public async Task<FullTimelineActivityDto?> CreateTimelineActivityAsync(CreateTimelineActivityDto dto, int currentUserId)
        {
            try
            {
                // Sprawdź czy kategoria istnieje (jeżeli została podana)
                Category? category = null;
                if (dto.CategoryId.HasValue)
                {
                    category = await _context.Categories.FindAsync(dto.CategoryId.Value);
                    if (category == null)
                        return null; // zwracamy null zamiast rzucać wyjątkiem
                }

                if (dto.PlannedDurationMinutes < 1 || dto.PlannedDurationMinutes > 1440) return null; // jescze w dto mozna dodac range 

                var entity = new TimelineActivity
                {
                    OwnerId = currentUserId,
                    Title = dto.Title,
                    Description = dto.Description,
                    CategoryId = dto.CategoryId,
                    Start_time = dto.StartTime,
                    End_time = dto.EndTime,
                    Is_recurring = dto.IsRecurring,
                    Recurrence_rule = dto.RecurrenceRule,
                    PlannedDurationMinutes = dto.PlannedDurationMinutes,
                    JoinCode = null
                };

                _context.TimelineActivities.Add(entity);
                await _context.SaveChangesAsync();

                //if (entity.Is_recurring && !string.IsNullOrEmpty(entity.Recurrence_rule))
                //{
                //    await _recurrenceService.GenerateInitialInstancesAsync(entity);
                //}

                if (!entity.Is_recurring)
                {
                    var instance = new TimelineRecurrenceInstance
                    {
                        ActivityId = entity.ActivityId,
                        OccurrenceDate = dto.StartTime.Date,
                        StartTime = dto.StartTime.TimeOfDay,
                        EndTime = dto.StartTime.AddMinutes(dto.PlannedDurationMinutes).TimeOfDay,
                        DurationMinutes = dto.PlannedDurationMinutes,
                        IsCompleted = false
                    };

                    _context.TimelineRecurrenceInstances.Add(instance);
                    await _context.SaveChangesAsync();
                }

                return new FullTimelineActivityDto
                {
                    ActivityId = entity.ActivityId,
                    Title = entity.Title,
                    Description = entity.Description,
                    StartTime = entity.Start_time,
                    EndTime = entity.End_time,
                    IsRecurring = entity.Is_recurring,
                    RecurrenceRule = entity.Recurrence_rule,
                    PlannedDurationMinutes = entity.PlannedDurationMinutes,
                    CategoryName = category?.Name
                };
            }
            catch
            {
                // Złap każdy błąd runtime i zwróć null
                return null;
            }
        }

        //PATCH Przekształć aktywność na PUBLICZNĄ
        public async Task<bool> ConvertToOnlineAsync(int activityId, int currentUserId)
        {
            var activity = await _context.TimelineActivities
                .FirstOrDefaultAsync(a => a.ActivityId == activityId && a.OwnerId == currentUserId);

            if (activity == null)
                return false; // brak dostępu lub nie istnieje

            if (activity.JoinCode != null)
                return true; // już jest online

            // 🔹 wygeneruj kod
            activity.JoinCode = GenerateJoinCode();

            // 🔹 dodaj ownera do ActivityMembers (jeśli nie istnieje)
            var ownerMemberExists = await _context.ActivityMembers
                .AnyAsync(m => m.ActivityId == activityId && m.UserId == currentUserId);

            if (!ownerMemberExists)
            {
                var ownerMember = new ActivityMembers
                {
                    ActivityId = activityId,
                    UserId = currentUserId,
                    Role = "owner",
                    Status = "accepted"
                };
                _context.ActivityMembers.Add(ownerMember);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        //PUT modyfikacja aktywności
        public async Task<FullTimelineActivityDto> UpdateTimelineActivityAsync (int activityId, int currentUserId, UpdateTimelineActivityDto dto)
        {
            var entity = await _context.TimelineActivities
                .FirstOrDefaultAsync(t => t.ActivityId == activityId && t.OwnerId == currentUserId);

            if (entity == null) return null;

            entity.Title = dto.Title;
            entity.Description = dto.Description;
            entity.CategoryId = dto.CategoryId;
            entity.Start_time = dto.StartTime;
            entity.End_time = dto.EndTime;
            entity.Is_recurring = dto.IsRecurring;
            entity.Recurrence_rule = dto.RecurrenceRule;
            //entity.Recurrence_exception = dto.RecurrenceException;

            if (!entity.Is_recurring)
                entity.PlannedDurationMinutes = dto.PlannedDurationMinutes;

            await _context.SaveChangesAsync();

            var category = await _context.Categories.FindAsync(dto.CategoryId);

            return new FullTimelineActivityDto
            {
                ActivityId = entity.ActivityId,
                Title = entity.Title,
                Description = entity.Description,
                StartTime = entity.Start_time,
                EndTime = entity.End_time,
                IsRecurring = entity.Is_recurring,
                RecurrenceRule = entity.Recurrence_rule,
                PlannedDurationMinutes = entity.PlannedDurationMinutes,
                CategoryName = category?.Name,
                JoinCode = entity.JoinCode
            };

        }

        ////PUT modyfikacja aktywności
        //public async Task<FullTimelineActivityDto> UpdateTimelineActivityAutomaticAsync(int activityId, int currentUserId, UpdateTimelineActivityDto dto)
        //{
        //    var entity = await _context.TimelineActivities
        //        .FirstOrDefaultAsync(t => t.ActivityId == activityId && t.OwnerId == currentUserId);

        //    if (entity == null) return null;

        //    entity.Title = dto.Title;
        //    entity.Description = dto.Description;
        //    entity.CategoryId = dto.CategoryId;
        //    entity.Start_time = dto.StartTime;
        //    entity.End_time = dto.EndTime;
        //    entity.Is_recurring = dto.IsRecurring;
        //    entity.Recurrence_rule = dto.RecurrenceRule;
        //    //entity.Recurrence_exception = dto.RecurrenceException;

        //    if (!entity.Is_recurring)
        //        entity.PlannedDurationMinutes = dto.PlannedDurationMinutes;

        //    await _context.SaveChangesAsync();

        //    var category = await _context.Categories.FindAsync(dto.CategoryId);

        //    return new FullTimelineActivityDto
        //    {
        //        ActivityId = entity.ActivityId,
        //        Title = entity.Title,
        //        Description = entity.Description,
        //        StartTime = entity.Start_time,
        //        EndTime = entity.End_time,
        //        IsRecurring = entity.Is_recurring,
        //        RecurrenceRule = entity.Recurrence_rule,
        //        PlannedDurationMinutes = entity.PlannedDurationMinutes,
        //        CategoryName = category?.Name,
        //        JoinCode = entity.JoinCode
        //    };

        //}



        //DELETE usunięcie aktywnosci
        public async Task<bool> DeleteTimelineActivityAsync(int activityId, int currentUserId)
        {
            var entity = await _context.TimelineActivities
                .FirstOrDefaultAsync(t => t.ActivityId == activityId && t.OwnerId == currentUserId);

            if (entity == null) return false;


            // Usuń zaproszenia związane z tą aktywnością
            var activityMembers = _context.ActivityMembers
                .Where(am => am.ActivityId == activityId);
            _context.ActivityMembers.RemoveRange(activityMembers);

            _context.TimelineActivities.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<TimelineActivityInstanceDto>> GetTimelineForUserAsync(int userId, DateTime from, DateTime to)
        {
            var activities = await _context.TimelineActivities
                .Include(a => a.Category)
                .Where(a => a.OwnerId == userId)
                .ToListAsync();

            var allInstances = new List<TimelineActivityInstanceDto>();

            //// 🧹 1️⃣ Auto-cleanup: usuń instancje wybiegające dalej niż 7 dni w przyszłość
            //var cleanupThreshold = DateTime.UtcNow.AddDays(7).Date;
            var cleanupThreshold = DateTime.UtcNow;

            var futureInstances = await _context.TimelineRecurrenceInstances
                .Where(i => i.OccurrenceDate > cleanupThreshold)
                .ToListAsync();


            if (futureInstances.Count > 0)
            {
                _context.TimelineRecurrenceInstances.RemoveRange(futureInstances);
                await _context.SaveChangesAsync();
            }

            // 🧠 2️⃣ Lazy loading: dogeneruj brakujące wystąpienia
            foreach (var activity in activities)
            {

                if (!activity.Is_recurring || string.IsNullOrEmpty(activity.Recurrence_rule))
                    continue;

                Console.WriteLine($"[ITER] {activity.ActivityId} | recurring={activity.Is_recurring} | rule={activity.Recurrence_rule}");

                var activityStart = activity.Start_time.Date;
                var activityEnd = activity.End_time?.Date ?? DateTime.UtcNow.AddMonths(1).Date;

                //przycinamy zakresy do granic aktywnosci
                var genFrom = from.Date < activityStart ? activityStart : from.Date;
                var genTo = to.Date > activityEnd ? activityEnd : to.Date;

                //pusty zakres - pomiń
                if (genTo < genFrom)
                    continue;

                var dto = new InstanceDto
                {
                    ActivityId = activity.ActivityId,
                    Start_time = genFrom,
                    End_time = genTo,
                    Is_recurring = true,
                    Recurrence_rule = activity.Recurrence_rule,
                    PlannedDurationMinutes = activity.PlannedDurationMinutes
                };
                await _recurrenceService.GenerateInstancesAsync(dto);
            }

            // 🔹 3️⃣ Pobierz wystąpienia w żądanym zakresie
            //var instances = await _context.TimelineRecurrenceInstances
            //    .Include(i => i.Activity)
            //    .ThenInclude(a => a.Category)
            //    .Where(i => i.Activity.OwnerId == userId && i.OccurrenceDate >= from.Date && i.OccurrenceDate <= to.Date)
            //    .ToListAsync();

            var instances = await _context.TimelineRecurrenceInstances
                .Include(i => i.Activity)
                .ThenInclude(a => a.Category)
                .Where(i => i.Activity.OwnerId == userId &&
                    i.OccurrenceDate >= from.Date &&
                    i.OccurrenceDate <= to.Date)
                .ToListAsync();

            foreach (var inst in instances)
            {
                allInstances.Add(new TimelineActivityInstanceDto
                {
                    ActivityId = inst.ActivityId,
                    Title = inst.Activity.Title,
                    StartTime = inst.OccurrenceDate.Date + inst.StartTime,
                    EndTime = inst.OccurrenceDate.Date + inst.StartTime.Add(TimeSpan.FromMinutes(inst.DurationMinutes)),
                    ColorHex = inst.Activity.Category?.ColorHex ?? "#3b82f6",
                    IsRecurring = inst.Activity.Is_recurring,
                    PlannedDurationMinutes = inst.DurationMinutes
                });
            }

            return allInstances.OrderBy(a => a.StartTime);
        }



        ////Wyświetlanie rekurencyjne
        //public async Task<IEnumerable<TimelineActivityInstanceDto>> GetTimelineForUserAsync(int userId, DateTime from, DateTime to)
        //{
        //    var activities = await _context.TimelineActivities
        //        .Include(a => a.Category)
        //        .Where(a => a.OwnerId == userId)
        //        .ToListAsync();

        //    var allInstances = new List<TimelineActivityInstanceDto>();

        //    foreach (var activity in activities)
        //    {
        //        if (activity.Is_recurring && !string.IsNullOrEmpty(activity.Recurrence_rule))
        //        {

        //            // start generacji to max(zakres_od, start_aktywności)
        //            var actualFrom = activity.Start_time > from ? activity.Start_time : from;

        //            var occurrences = _recurrenceService.GenerateOccurrences(
        //            actualFrom,
        //            activity.Recurrence_rule,
        //            activity.Recurrence_exception,
        //            (int)(to - actualFrom).TotalDays + 30,
        //            activity.End_time
        //            );

        //            foreach (var occurrence in occurrences)
        //            {

        //                var customDur = _recurrenceService.GetExceptionDuration(occurrence.Date);

        //                var durationMinutes = customDur ??
        //                    (activity.PlannedDurationMinutes > 0 ? activity.PlannedDurationMinutes : 60);
        //                if (occurrence >= from && occurrence <= to)
        //                {
        //                    allInstances.Add(new TimelineActivityInstanceDto
        //                    {
        //                        ActivityId = activity.ActivityId,
        //                        Title = activity.Title,
        //                        StartTime = occurrence,
        //                        EndTime = occurrence.AddMinutes(durationMinutes),
        //                        ColorHex = activity.Category?.ColorHex ?? "#3b82f6",
        //                        IsRecurring = true,
        //                        PlannedDurationMinutes = durationMinutes
        //                    });
        //                }
        //                else if (activity.End_time == null && occurrence <= to)
        //                {
        //                    allInstances.Add(new TimelineActivityInstanceDto
        //                    {
        //                        ActivityId = activity.ActivityId,
        //                        Title = activity.Title,
        //                        StartTime = occurrence,
        //                        EndTime = occurrence.AddMinutes(durationMinutes),
        //                        //occurrence.AddMinutes(activity.PlannedDurationMinutes > 0
        //                        //    ? activity.PlannedDurationMinutes
        //                        //    : 60),
        //                        ColorHex = activity.Category?.ColorHex ?? "#3b82f6",
        //                        IsRecurring = true,
        //                        PlannedDurationMinutes = durationMinutes
        //                    });
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var endTime = activity.End_time ?? activity.Start_time.AddMinutes(
        //                activity.PlannedDurationMinutes > 0 ? activity.PlannedDurationMinutes : 60);

        //            if (activity.Start_time <= to && endTime >= from)
        //            {

        //                var startUtc = DateTime.SpecifyKind(activity.Start_time, DateTimeKind.Utc);
        //                var endUtc   = DateTime.SpecifyKind(endTime, DateTimeKind.Utc);

        //                allInstances.Add(new TimelineActivityInstanceDto
        //                {
        //                    ActivityId = activity.ActivityId,
        //                    Title = activity.Title,
        //                    StartTime = startUtc,
        //                    EndTime = endUtc,
        //                    ColorHex = activity.Category?.ColorHex ?? "#3b82f6",
        //                    IsRecurring = false,
        //                    PlannedDurationMinutes = activity.PlannedDurationMinutes
        //                });
        //            }
        //        }
        //    }

        //    return allInstances.OrderBy(a => a.StartTime);
        //}


        //public async Task<FullTimelineActivityDto?> AdjustTimelineAsync(ActivityModificationSuggestionDto dto, int userId)
        //{
        //    var entity = await _context.TimelineActivities
        //        .FirstOrDefaultAsync(t => t.ActivityId == dto.ActivityId && t.OwnerId == userId);

        //    if (entity == null)
        //        return null;

        //    UpdateTimelineActivityDto updateDto;

        //    if (entity.Is_recurring == true)
        //        updateDto = BuildRecurrenceUpdateDto(entity, dto);
        //    else
        //        updateDto = BuildSingleUpdateDto(entity, dto);

        //    return await UpdateTimelineActivityAutomaticAsync(dto.ActivityId, userId, updateDto);
        //}


        //private UpdateTimelineActivityDto BuildSingleUpdateDto(TimelineActivity entity, ActivityModificationSuggestionDto dto)
        //{
        //    // dla jednorazowych — modyfikujemy czas startu / końca + przeliczamy długość
        //    var plannedMinutes = (int)(dto.NewEndTime.Value - dto.NewStartTime).TotalMinutes;

        //    return new UpdateTimelineActivityDto
        //    {
        //        Title = entity.Title,
        //        Description = entity.Description,
        //        CategoryId = entity.CategoryId,
        //        StartTime = dto.NewStartTime,
        //        EndTime = dto.NewEndTime,
        //        IsRecurring = false,
        //        RecurrenceRule = entity.Recurrence_rule,
        //        RecurrenceException = entity.Recurrence_exception,
        //        PlannedDurationMinutes = plannedMinutes
        //    };
        //}

        //private UpdateTimelineActivityDto BuildRecurrenceUpdateDto(TimelineActivity entity, ActivityModificationSuggestionDto dto)
        //{
        //    var plannedMinutes = (int)(dto.NewEndTime.Value - dto.NewStartTime).TotalMinutes;

        //    // generujemy zaktualizowany wpis RecurrenceException (np. "20251110@TIMES=15:00|DUR=180")
        //    var newException = UpsertRecurrenceException(
        //        entity.Recurrence_exception,
        //        dto.NewStartTime.Date,
        //        dto.NewStartTime.TimeOfDay,
        //        plannedMinutes
        //    );

        //    return new UpdateTimelineActivityDto
        //    {
        //        Title = entity.Title,
        //        Description = entity.Description,
        //        CategoryId = entity.CategoryId,
        //        StartTime = entity.Start_time,         // bez zmian
        //        EndTime = entity.End_time,             // bez zmian
        //        IsRecurring = true,
        //        RecurrenceRule = entity.Recurrence_rule,
        //        RecurrenceException = newException,    // zaktualizowany wyjątek
        //        PlannedDurationMinutes = entity.PlannedDurationMinutes
        //    };
        //}


        private static string UpsertRecurrenceException(
    string? currentExceptions,
    DateTime date,           // np. 2025-11-06
    TimeSpan timeOfDay,      // np. 15:00
    int? durationMinutes = null // na razie ignorowany jeśli brak
)
        {
            // Słownik (klucz = data w formacie yyyyMMdd)
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // 1️⃣ Wczytaj istniejące wyjątki
            if (!string.IsNullOrWhiteSpace(currentExceptions))
            {
                foreach (var token in currentExceptions.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    var trimmed = token.Trim();
                    var at = trimmed.IndexOf('@');
                    if (at <= 0) continue;

                    var key = trimmed.Substring(0, at); // yyyyMMdd
                    map[key] = trimmed;                 // pełny wpis
                }
            }

            // 2️⃣ Zbuduj nowy wpis dla podanej daty
            var keyDate = date.ToString("yyyyMMdd");
            var hhmm = $"{timeOfDay:hh\\:mm}"; // 15:00

            string newEntry;

            if (durationMinutes.HasValue)
                newEntry = $"{keyDate}@TIMES={hhmm}|DUR={durationMinutes.Value}";
            else
                newEntry = $"{keyDate}@TIMES={hhmm}";

            // 3️⃣ Dodaj lub nadpisz
            map[keyDate] = newEntry;

            // 4️⃣ Połącz z powrotem w string
            var result = string.Join(";", map.Values.OrderBy(v => v, StringComparer.Ordinal));

            return result;
        }


        private static string GenerateJoinCode(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
        }




        //public async Task<FullTimelineActivityDto?> PlaceActivityAsync(int userId, ActivityPlacementDto dto)
        //{
        //    var entity = await _context.TimelineActivities
        //        .Include(a => a.Category)
        //        .FirstOrDefaultAsync(t => t.ActivityId == dto.ActivityId && t.OwnerId == userId);

        //    if (entity == null)
        //        return null;

        //    if (entity.Is_recurring)
        //    {
        //        // 🔹 Dopisanie wyjątku z nowym terminem (i ewentualnie DUR)
        //        entity.Recurrence_exception = UpsertRecurrenceException(
        //            entity.Recurrence_exception,
        //            dto.SuggestedStart,
        //            dto.SuggestedStart.TimeOfDay,
        //            dto.DurationMinutes
        //        );
        //    }
        //    else
        //    {
        //        // 🔹 Aktywność jednorazowa – zmiana daty i długości
        //        entity.Start_time = dto.SuggestedStart;
        //        entity.End_time = dto.SuggestedStart.AddMinutes(dto.DurationMinutes);
        //        entity.PlannedDurationMinutes = dto.DurationMinutes;
        //    }

        //    await _context.SaveChangesAsync();

        //    return new FullTimelineActivityDto
        //    {
        //        ActivityId = entity.ActivityId,
        //        Title = entity.Title,
        //        Description = entity.Description,
        //        StartTime = entity.Start_time,
        //        EndTime = entity.End_time,
        //        IsRecurring = entity.Is_recurring,
        //        RecurrenceRule = entity.Recurrence_rule,
        //        PlannedDurationMinutes = entity.PlannedDurationMinutes,
        //        CategoryName = entity.Category?.Name
        //    };
        //}



    }
}
