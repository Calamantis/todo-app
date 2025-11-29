using Microsoft.EntityFrameworkCore;
using todo_backend.Data;
using todo_backend.Dtos.ActivityInstance;
using todo_backend.Models;

namespace todo_backend.Services.ActivityInstanceService
{
    public class ActivityInstanceService : IActivityInstanceService
    {
        private readonly AppDbContext _context;

        public ActivityInstanceService(AppDbContext context)
        {
            _context = context;
        }

        // GET: Wszystkie instancje użytkownika
        public async Task<IEnumerable<ActivityInstanceDto>> GetAllInstancesByUserIdAsync(int userId)
        {
            return await _context.ActivityInstances
                .Where(ai => ai.Activity.OwnerId == userId) // Sprawdzenie, czy instancje należą do użytkownika
                .Select(ai => new ActivityInstanceDto
                {
                    InstanceId = ai.InstanceId,
                    ActivityId = ai.ActivityId,
                    UserId = ai.UserId,
                    RecurrenceRuleId = ai.RecurrenceRuleId,
                    OccurrenceDate = ai.OccurrenceDate,
                    StartTime = ai.StartTime,
                    EndTime = ai.EndTime,
                    DurationMinutes = ai.DurationMinutes,
                    IsActive = ai.IsActive,
                    DidOccur = ai.DidOccur,
                    IsException = ai.IsException
                })
                .ToListAsync();
        }

        // GET: Instancje po ID aktywności
        public async Task<IEnumerable<ActivityInstanceDto>> GetInstancesByActivityIdAsync(int activityId, int userId)
        {

            var now = DateTime.Now;
            var oneDayAgo = now.AddDays(-1);

            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.ActivityId == activityId && a.OwnerId == userId);

            if (activity == null)
            {
                return null; // Jeśli aktywność nie należy do użytkownika, zwróć null
            }

            return await _context.ActivityInstances
                .Where(ai => ai.ActivityId == activityId && ai.OccurrenceDate > oneDayAgo)
                .Select(ai => new ActivityInstanceDto
                {
                    InstanceId = ai.InstanceId,
                    ActivityId = ai.ActivityId,
                    UserId = ai.UserId,
                    RecurrenceRuleId = ai.RecurrenceRuleId,
                    OccurrenceDate = ai.OccurrenceDate,
                    StartTime = ai.StartTime,
                    EndTime = ai.EndTime,
                    DurationMinutes = ai.DurationMinutes,
                    IsActive = ai.IsActive,
                    DidOccur = ai.DidOccur,
                    IsException = ai.IsException
                })
                .ToListAsync();
        }

        //// GET: Instancje w określonym przedziale dat
        //public async Task<IEnumerable<ActivityInstanceDto>> GetInstancesByDateRangeAsync(DateTime startDate, DateTime endDate, int userId)
        //{
        //    return await _context.ActivityInstances
        //        .Where(ai => ai.Activity.OwnerId == userId && ai.OccurrenceDate >= startDate && ai.OccurrenceDate <= endDate)
        //        .Select(ai => new ActivityInstanceDto
        //        {
        //            InstanceId = ai.InstanceId,
        //            ActivityId = ai.ActivityId,
        //            RecurrenceRuleId = ai.RecurrenceRuleId,
        //            OccurrenceDate = ai.OccurrenceDate,
        //            StartTime = ai.StartTime,
        //            EndTime = ai.EndTime,
        //            DurationMinutes = ai.DurationMinutes,
        //            IsActive = ai.IsActive,
        //            DidOccur = ai.DidOccur,
        //            IsException = ai.IsException
        //        })
        //        .ToListAsync();
        //}

        // POST: Tworzenie nowej instancji
        //public async Task<ActivityInstanceDto?> CreateInstanceAsync(ActivityInstanceDto dto, int userId)
        //{
        //    var activity = await _context.Activities
        //        .FirstOrDefaultAsync(a => a.ActivityId == dto.ActivityId && a.OwnerId == userId);

        //    if (activity == null)
        //    {
        //        return null; // Jeśli aktywność nie należy do użytkownika, zwróć null
        //    }

        //    var entity = new ActivityInstance
        //    {
        //        ActivityId = dto.ActivityId,
        //        UserId = userId,
        //        RecurrenceRuleId = dto.RecurrenceRuleId,
        //        OccurrenceDate = dto.OccurrenceDate,
        //        StartTime = dto.StartTime,
        //        EndTime = dto.EndTime,
        //        DurationMinutes = dto.DurationMinutes,
        //        IsActive = dto.IsActive,
        //        DidOccur = dto.DidOccur,
        //        IsException = dto.IsException
        //    };

        //    _context.ActivityInstances.Add(entity);
        //    await _context.SaveChangesAsync();

        //    return new ActivityInstanceDto
        //    {
        //        InstanceId = entity.InstanceId,
        //        ActivityId = entity.ActivityId,
        //        RecurrenceRuleId = entity.RecurrenceRuleId,
        //        UserId = entity.UserId,
        //        OccurrenceDate = entity.OccurrenceDate,
        //        StartTime = entity.StartTime,
        //        EndTime = entity.EndTime,
        //        DurationMinutes = entity.DurationMinutes,
        //        IsActive = entity.IsActive,
        //        DidOccur = entity.DidOccur,
        //        IsException = entity.IsException
        //    };
        //}

        public async Task<ActivityInstanceDto?> CreateInstanceAsync(ActivityInstanceDto dto, int userId)
        {
            // 1) Pobieramy aktywność BEZ sprawdzania ownera
            var activity = await _context.Activities
                .Include(a => a.Category)
                .FirstOrDefaultAsync(a => a.ActivityId == dto.ActivityId);

            if (activity == null)
            {
                Console.WriteLine(dto);
            Console.WriteLine(dto.ActivityId);
            Console.WriteLine("siema, zwracam null hihi");
            return null;
            }

            // 2) Jeśli aktywność nie należy do użytkownika → klonujemy
            if (activity.OwnerId != userId)
            {
                var clone = new Activity
                {
                    OwnerId = userId,
                    Title = activity.Title,
                    Description = activity.Description,
                    IsRecurring = false,   // instancje są zawsze jednorazowe
                    CategoryId = null, //jak na razie null nie chce mi sie kopiowac jeszcze kategorii
                    JoinCode = null,
                    isFriendsOnly = false
                };

                _context.Activities.Add(clone);
                await _context.SaveChangesAsync();

                // podmieniamy activity na klona
                activity = clone;
            }

            // 3) Tworzymy instancję dla właściciela
            var instance = new ActivityInstance
            {
                ActivityId = activity.ActivityId,   // <- ZAWSZE poprawne ID
                UserId = userId,
                RecurrenceRuleId = null,
                OccurrenceDate = dto.OccurrenceDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                DurationMinutes = dto.DurationMinutes,
                IsActive = dto.IsActive,
                DidOccur = dto.DidOccur,
                IsException = dto.IsException
            };

            _context.ActivityInstances.Add(instance);
            await _context.SaveChangesAsync();

            return new ActivityInstanceDto
            {
                InstanceId = instance.InstanceId,
                ActivityId = instance.ActivityId,
                UserId = instance.UserId,
                OccurrenceDate = instance.OccurrenceDate,
                StartTime = instance.StartTime,
                EndTime = instance.EndTime,
                DurationMinutes = instance.DurationMinutes,
                IsActive = instance.IsActive,
                DidOccur = instance.DidOccur,
                IsException = instance.IsException
            };
        }


        // PUT: Aktualizacja instancji
        public async Task<ActivityInstanceDto?> UpdateInstanceAsync(int instanceId, ActivityInstanceDto dto, int userId)
        {
            var instance = await _context.ActivityInstances
                .Include(ai => ai.Activity)
                .FirstOrDefaultAsync(ai => ai.InstanceId == instanceId);

            if (instance == null || instance.Activity.OwnerId != userId)
            {
                return null; // Jeśli instancja nie należy do użytkownika, zwróć null
            }

            instance.OccurrenceDate = dto.OccurrenceDate;
            instance.StartTime = dto.StartTime;
            instance.UserId = userId;
            instance.EndTime = dto.EndTime;
            instance.DurationMinutes = dto.DurationMinutes;
            instance.IsActive = dto.IsActive;
            instance.DidOccur = dto.DidOccur;
            instance.IsException = dto.IsException;

            await _context.SaveChangesAsync();

            return new ActivityInstanceDto
            {
                InstanceId = instance.InstanceId,
                ActivityId = instance.ActivityId,
                RecurrenceRuleId = instance.RecurrenceRuleId,
                UserId = userId,
                OccurrenceDate = instance.OccurrenceDate,
                StartTime = instance.StartTime,
                EndTime = instance.EndTime,
                DurationMinutes = instance.DurationMinutes,
                IsActive = instance.IsActive,
                DidOccur = instance.DidOccur,
                IsException = instance.IsException
            };
        }

        // DELETE: Usuwanie instancji
        //public async Task<bool> DeleteInstanceAsync(int instanceId, int userId)
        //{
        //    var instance = await _context.ActivityInstances.FirstOrDefaultAsync(ai => ai.InstanceId == instanceId);

        //    Console.WriteLine("\n\n"+instance);

        //    if (instance == null || instance.UserId != userId)
        //    {
        //        return false; // Jeśli instancja nie należy do użytkownika, zwróć false
        //    }

        //    _context.ActivityInstances.Remove(instance);
        //    await _context.SaveChangesAsync();

        //    return true;
        //}

        public async Task<bool> DeleteInstanceAsync(int instanceId, int userId)
        {
            var instance = await _context.ActivityInstances
                .FirstOrDefaultAsync(ai => ai.InstanceId == instanceId);

            if (instance == null || instance.UserId != userId)
            {
                return false;
            }

            var now = DateTime.Now;

            // 1) PRZYSZŁA INSTANCJA + MA REGUŁĘ?
            if (instance.OccurrenceDate > now && instance.RecurrenceRuleId.HasValue)
            {
                // ➜ Dodajemy rekord exclusion zamiast usuwać instancję
                var exclusion = new InstanceExclusion
                {
                    UserId = userId,
                    ActivityId = instance.ActivityId,
                    ExcludedDate = instance.OccurrenceDate.Date,
                    StartTime = instance.StartTime,
                    EndTime = instance.EndTime

                };

                _context.InstanceExclusions.Add(exclusion);

                // Instancji NIE usuwamy — generator będzie wiedział, aby jej nie utworzyć
            }
            else
            {
                // 2) PRZESZŁA INSTANCJA LUB BRAK REGUŁY
                //    ➜ Usuwamy normalnie
                _context.ActivityInstances.Remove(instance);
            }

            await _context.SaveChangesAsync();
            return true;
        }



        public async Task<InstanceParticipantsResponseDto?> GetInstanceParticipantsAsync(
            int ownerId,
            int activityId,
            int instanceId)
        {
            // 1. Pobierz instancję + aktywność
            var instance = await _context.ActivityInstances
                .Include(i => i.Activity)
                .FirstOrDefaultAsync(i =>
                    i.InstanceId == instanceId &&
                    i.ActivityId == activityId);

            if (instance == null)
                return null;

            //// 2. Sprawdź, czy proszący user jest ownerem tej aktywności
            //if (instance.Activity.OwnerId != ownerId)
            //    return null; // możesz tu ewentualnie rzucić Forbidden w kontrolerze

            // 3. Pobierz wszystkich członków aktywności (owner + participants)
            var members = await _context.ActivityMembers
                .Include(am => am.User)
                .Where(am => am.ActivityId == activityId &&
                             (
                                 am.Role == "owner" ||
                                 am.Role == "participant"
                             ) &&
                             (
                                 am.Status == "accepted" || am.Role == "owner"
                             ))
                .ToListAsync();

            // 4. Pobierz wykluczenia dotyczące TEJ daty
            var exclusions = await _context.InstanceExclusions
                .Where(e =>
                    e.ActivityId == activityId &&
                    e.ExcludedDate.Date == instance.OccurrenceDate.Date &&
                    e.StartTime == instance.StartTime )
                .ToListAsync();

            foreach (var exclusion in exclusions )
            Console.WriteLine("\n\n" + exclusions + "\n\n");

            // 5. Złóż odpowiedź
            var response = new InstanceParticipantsResponseDto
            {
                ActivityId = activityId,
                InstanceId = instanceId,
                OccurrenceDate = instance.OccurrenceDate,
                StartTime = instance.StartTime,
                EndTime = instance.EndTime
            };

            foreach (var m in members)
            {
                // sprawdzamy, czy user ma exclusion nachodzące na ten przedział godzin
                bool isExcluded = exclusions.Any(e =>
                    e.UserId == m.UserId &&
                    e.StartTime <= instance.EndTime &&
                    e.EndTime >= instance.StartTime);

                if (isExcluded)
                    continue;

                response.Participants.Add(new InstanceParticipantDto
                {
                    UserId = m.UserId,
                    Username = m.User.FullName,
                    Email = m.User.Email,
                    Role = m.Role,
                    IsAttending = !isExcluded
                });
            }

            return response;
        }
    }



}