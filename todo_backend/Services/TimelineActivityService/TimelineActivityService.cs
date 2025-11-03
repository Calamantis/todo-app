using Microsoft.EntityFrameworkCore;

using todo_backend.Data;
using todo_backend.Dtos.Friendship;
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

                //// 🔹 Dodaj ownera do ActivityMembers
                //var ownerMember = new ActivityMembers
                //{
                //    ActivityId = entity.ActivityId,
                //    UserId = currentUserId,
                //    Role = "owner",
                //    Status = "accepted" // automatycznie zaakceptowany
                //};
                //_context.ActivityMembers.Add(ownerMember);
                //await _context.SaveChangesAsync();

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

        // // Pod wyswietlanie rekurencyjne
        // public async Task<IEnumerable<TimelineActivityInstanceDto>> GetTimelineForUserAsync(int userId, int daysAhead)
        // {
        //     //var activities = await _context.TimelineActivities
        //     //    .Include(a => a.Category)
        //     //    .Where(a => a.OwnerId == userId)
        //     //    .ToListAsync();

        //     //var allInstances = new List<TimelineActivityInstanceDto>();
        //     //var now = DateTime.UtcNow;

        //     //foreach (var activity in activities)
        //     //{
        //     //    // 🔸 Jeśli aktywność ma regułę powtarzalności → generuj wystąpienia
        //     //    if (activity.Is_recurring && !string.IsNullOrEmpty(activity.Recurrence_rule))
        //     //    {
        //     //        var occurrences = _recurrenceService.GenerateOccurrences(
        //     //            activity.Start_time,
        //     //            activity.Recurrence_rule,
        //     //            daysAhead
        //     //        );

        //     //        foreach (var occurrence in occurrences)
        //     //        {
        //     //            // Filtrujemy tylko przyszłe i bieżące zdarzenia
        //     //            if (occurrence >= now.AddDays(-1) || activity.End_time == null)
        //     //            {
        //     //                allInstances.Add(new TimelineActivityInstanceDto
        //     //                {
        //     //                    ActivityId = activity.ActivityId,
        //     //                    Title = activity.Title,
        //     //                    StartTime = occurrence,
        //     //                    EndTime = activity.End_time != null
        //     //                        ? occurrence.Add(activity.End_time.Value - activity.Start_time)
        //     //                        : occurrence.AddHours(1),
        //     //                    ColorHex = activity.Category?.ColorHex ?? "#3b82f6",
        //     //                    IsRecurring = true,
        //     //                    PlannedDurationMinutes = activity.PlannedDurationMinutes
        //     //                });
        //     //            }
        //     //        }
        //     //    }
        //     //    else
        //     //    {
        //     //        // 🔸 Zwykła (jednorazowa) aktywność
        //     //        allInstances.Add(new TimelineActivityInstanceDto
        //     //        {
        //     //            ActivityId = activity.ActivityId,
        //     //            Title = activity.Title,
        //     //            StartTime = activity.Start_time,
        //     //            EndTime = activity.End_time,
        //     //            ColorHex = activity.Category?.ColorHex ?? "#3b82f6",
        //     //            IsRecurring = false,
        //     //            PlannedDurationMinutes = activity.PlannedDurationMinutes
        //     //        });


        //     //var endTime = activity.End_time ?? activity.Start_time.AddMinutes(activity.PlannedDurationMinutes > 0
        //     //    ? activity.PlannedDurationMinutes
        //     //    : 60);

        //     //// 🔹 Jeśli aktywność nie ma końca — pokaż ją zawsze
        //     //if (endTime >= now || activity.End_time == null)
        //     //{
        //     //    allInstances.Add(new TimelineActivityInstanceDto
        //     //    {
        //     //        ActivityId = activity.ActivityId,
        //     //        Title = activity.Title,
        //     //        StartTime = activity.Start_time,
        //     //        EndTime = endTime,
        //     //        ColorHex = activity.Category?.ColorHex ?? "#3b82f6",
        //     //        IsRecurring = false,
        //     //        PlannedDurationMinutes = activity.PlannedDurationMinutes
        //     //    });
        //     //}

        //     //}
        //     //}
        //     // // 🔹 Sortujemy po dacie rozpoczęcia
        //     //return allInstances.OrderBy(a => a.StartTime);

        //     var activities = await _context.TimelineActivities
        //.Include(a => a.Category)
        //.Where(a => a.OwnerId == userId)
        //.ToListAsync();

        //     var allInstances = new List<TimelineActivityInstanceDto>();
        //     var now = DateTime.UtcNow;
        //     var endDate = now.AddDays(daysAhead);

        //     foreach (var activity in activities)
        //     {
        //         if (activity.Is_recurring && !string.IsNullOrEmpty(activity.Recurrence_rule))
        //         {
        //             var occurrences = _recurrenceService.GenerateOccurrences(
        //                 activity.Start_time,
        //                 activity.Recurrence_rule,
        //                 daysAhead + 60 // lekkie nadmiarowe generowanie
        //             );

        //             foreach (var occurrence in occurrences)
        //             {
        //                 // 🔹 pokaż tylko wystąpienia od teraz do końca zakresu
        //                 if (occurrence >= now && occurrence <= endDate)
        //                 {
        //                     allInstances.Add(new TimelineActivityInstanceDto
        //                     {
        //                         ActivityId = activity.ActivityId,
        //                         Title = activity.Title,
        //                         StartTime = occurrence,
        //                         EndTime = occurrence.AddMinutes(activity.PlannedDurationMinutes > 0
        //                             ? activity.PlannedDurationMinutes
        //                             : 60),
        //                         ColorHex = activity.Category?.ColorHex ?? "#3b82f6",
        //                         IsRecurring = true,
        //                         PlannedDurationMinutes = activity.PlannedDurationMinutes
        //                     });
        //                 }
        //                 // 🔹 Dodatkowy warunek: aktywność bez końca też ma się pojawiać
        //                 else if (activity.End_time == null && occurrence <= endDate)
        //                 {
        //                     allInstances.Add(new TimelineActivityInstanceDto
        //                     {
        //                         ActivityId = activity.ActivityId,
        //                         Title = activity.Title,
        //                         StartTime = occurrence,
        //                         EndTime = occurrence.AddMinutes(activity.PlannedDurationMinutes > 0
        //                             ? activity.PlannedDurationMinutes
        //                             : 60),
        //                         ColorHex = activity.Category?.ColorHex ?? "#3b82f6",
        //                         IsRecurring = true,
        //                         PlannedDurationMinutes = activity.PlannedDurationMinutes
        //                     });
        //                 }
        //             }
        //         }
        //         else
        //         {
        //             // jednorazowe aktywności
        //             var endTime = activity.End_time ?? activity.Start_time.AddMinutes(
        //                 activity.PlannedDurationMinutes > 0 ? activity.PlannedDurationMinutes : 60);

        //             if (endTime >= now && activity.Start_time <= endDate)
        //             {
        //                 allInstances.Add(new TimelineActivityInstanceDto
        //                 {
        //                     ActivityId = activity.ActivityId,
        //                     Title = activity.Title,
        //                     StartTime = activity.Start_time,
        //                     EndTime = endTime,
        //                     ColorHex = activity.Category?.ColorHex ?? "#3b82f6",
        //                     IsRecurring = false,
        //                     PlannedDurationMinutes = activity.PlannedDurationMinutes
        //                 });
        //             }
        //         }
        //     }

        //     return allInstances.OrderBy(a => a.StartTime);


        // }



        public async Task<IEnumerable<TimelineActivityInstanceDto>> GetTimelineForUserAsync(int userId, DateTime from, DateTime to)
        {
            var activities = await _context.TimelineActivities
                .Include(a => a.Category)
                .Where(a => a.OwnerId == userId)
                .ToListAsync();

            var allInstances = new List<TimelineActivityInstanceDto>();

            foreach (var activity in activities)
            {
                if (activity.Is_recurring && !string.IsNullOrEmpty(activity.Recurrence_rule))
                {
                    var occurrences = _recurrenceService.GenerateOccurrences(
                        from,
                        activity.Recurrence_rule,
                        (int)(to - from).TotalDays + 30 // bufor
                    );

                    foreach (var occurrence in occurrences)
                    {
                        if (occurrence >= from && occurrence <= to)
                        {
                            allInstances.Add(new TimelineActivityInstanceDto
                            {
                                ActivityId = activity.ActivityId,
                                Title = activity.Title,
                                StartTime = occurrence,
                                EndTime = occurrence.AddMinutes(activity.PlannedDurationMinutes > 0
                                    ? activity.PlannedDurationMinutes
                                    : 60),
                                ColorHex = activity.Category?.ColorHex ?? "#3b82f6",
                                IsRecurring = true,
                                PlannedDurationMinutes = activity.PlannedDurationMinutes
                            });
                        }
                        else if (activity.End_time == null && occurrence <= to)
                        {
                            allInstances.Add(new TimelineActivityInstanceDto
                            {
                                ActivityId = activity.ActivityId,
                                Title = activity.Title,
                                StartTime = occurrence,
                                EndTime = occurrence.AddMinutes(activity.PlannedDurationMinutes > 0
                                    ? activity.PlannedDurationMinutes
                                    : 60),
                                ColorHex = activity.Category?.ColorHex ?? "#3b82f6",
                                IsRecurring = true,
                                PlannedDurationMinutes = activity.PlannedDurationMinutes
                            });
                        }
                    }
                }
                else
                {
                    var endTime = activity.End_time ?? activity.Start_time.AddMinutes(
                        activity.PlannedDurationMinutes > 0 ? activity.PlannedDurationMinutes : 60);

                    if (activity.Start_time <= to && endTime >= from)
                    {
                        allInstances.Add(new TimelineActivityInstanceDto
                        {
                            ActivityId = activity.ActivityId,
                            Title = activity.Title,
                            StartTime = activity.Start_time,
                            EndTime = endTime,
                            ColorHex = activity.Category?.ColorHex ?? "#3b82f6",
                            IsRecurring = false,
                            PlannedDurationMinutes = activity.PlannedDurationMinutes
                        });
                    }
                }
            }

            return allInstances.OrderBy(a => a.StartTime);
        }



        private static string GenerateJoinCode(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
        }

    }
}
