using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using todo_backend.Data;
using todo_backend.Dtos.ActivityMembers;
using todo_backend.Models;

namespace todo_backend.Services.ActivityMembersService
{
    public class ActivityMembersService : IActivityMembersService
    {
        private readonly AppDbContext _context;

        public ActivityMembersService(AppDbContext context)
        {
            _context = context;
        }


        //GET przeglądaj dostane zaproszenia
        public async Task<IEnumerable<ActivityInviteDto?>> GetSentInvitesAsync(int userId)
        {
            var sentInvites = await _context.ActivityMembers
                           .Where(am => am.UserId == userId && am.Status == "pending")
                           .Include(am => am.Activity)
                           .ThenInclude(a => a.User)
                           .Select(am => new ActivityInviteDto
                           {
                               ActivityId = am.ActivityId,
                               ActivityTitle = am.Activity.Title,
                               OwnerFullName = am.Activity.User.FullName,
                               Status = am.Status
                           })
                           .ToListAsync();

            return sentInvites;
        }

        //GET przeglądaj zaakceptowane zaproszenia
        public async Task<IEnumerable<ActivityInviteDto?>> GetAcceptedInvitesAsync(int userId)
        {
            var sentInvites = await _context.ActivityMembers
                           .Where(am => am.UserId == userId && am.Status == "accepted")
                           .Include(am => am.Activity)
                           .ThenInclude(a => a.User)
                           .Select(am => new ActivityInviteDto
                           {
                               ActivityId = am.ActivityId,
                               ActivityTitle = am.Activity.Title,
                               OwnerFullName = am.Activity.User.FullName,
                               Status = am.Status
                           })
                           .ToListAsync();


            return sentInvites;
        }


        //GET przeglądaj wysłane zaproszenia
        public async Task<IEnumerable<FullActivityMembersDto?>> GetSentInvitesAsync(int activityId, int requestingUserId)
        {
            var isParticipantOrOwner = await _context.TimelineActivities
                .AnyAsync(a => a.ActivityId == activityId && a.OwnerId == requestingUserId)
                ||
                 await _context.ActivityMembers
                 .AnyAsync(am => am.ActivityId == activityId && am.UserId == requestingUserId);

            if (!isParticipantOrOwner)
                return null; // lub throw new UnauthorizedAccessException();


            var participants = await _context.ActivityMembers
                            .Where(am => am.ActivityId == activityId && am.Status == "pending")
                            .Include(am => am.User)
                            .Select(am => new FullActivityMembersDto
                            {
                                ActivityId = am.ActivityId,
                                UserId = am.UserId,
                                Role = am.Role,
                                Status = am.Status,
                                UserFullName = am.User.FullName,
                                UserEmail = am.User.Email
                            })
                            .ToListAsync();

            return participants;
        }

        //GET zaakceptowane zaproszenia
        public async Task<IEnumerable<FullActivityMembersDto?>> GetParticipantsOfActivityAsync(int activityId, int requestingUserId)
        {
            var isParticipantOrOwner = await _context.TimelineActivities
                .AnyAsync(a => a.ActivityId == activityId && a.OwnerId == requestingUserId)
                 ||
                 await _context.ActivityMembers
                   .AnyAsync(am => am.ActivityId == activityId && am.UserId == requestingUserId);

            if (!isParticipantOrOwner)
                return null; // lub throw new UnauthorizedAccessException();


            var participants = await _context.ActivityMembers
                .Where(am => am.ActivityId == activityId && am.Status == "accepted" || am.Status == "auto_accepted")
                .Include(am => am.User)
                .Select(am => new FullActivityMembersDto
                {
                    ActivityId = am.ActivityId,
                    UserId = am.UserId,
                    Role = am.Role,
                    Status = am.Status,
                    UserFullName = am.User.FullName,
                    UserEmail = am.User.Email
                })
                .ToListAsync();

            return participants;
        }

        //POST wyslanie zaproszenia
        public async Task<ActivityInviteDto?> SendInviteAsync(int activityId, int ownerId, int invitedUserId)
        {
            // Nie można zaprosić samego siebie
            if (ownerId == invitedUserId) return null;

            var activity = await _context.TimelineActivities
                .Include(a => a.User) // wczytanie ownera
                .FirstOrDefaultAsync(a => a.ActivityId == activityId);
            if (activity == null || activity.OwnerId != ownerId) return null;

            // Sprawdź czy osoba zapraszaja jest w znajomych
            var isFriend = await _context.Friendships
                .AnyAsync(f =>
                ((f.UserId == ownerId && f.FriendId == invitedUserId) ||
                (f.UserId == invitedUserId && f.FriendId == ownerId)) &&
                f.Status == "accepted");
            if (!isFriend) return null; // jeśli nie jest znajomym, zwracamy false

            // Sprawdź czy taki rekord już istnieje
            var exists = await _context.ActivityMembers
                .AnyAsync(am => am.ActivityId == activityId && am.UserId == invitedUserId);
            if (exists) return null;

            var activityMember = new ActivityMembers
            {
                ActivityId = activityId,
                UserId = invitedUserId,
                Role = "participant",
                Status = "pending"
            };

            _context.ActivityMembers.Add(activityMember);
            await _context.SaveChangesAsync();

            return new ActivityInviteDto
            {
                ActivityId = activity.ActivityId,
                InvitedUserId = invitedUserId,
                ActivityTitle = activity.Title,
                OwnerFullName = activity.User.FullName,
                Status = activityMember.Status
            };
        }

        //POST dolaczenie po kodzie
        public async Task<bool> JoinActivityByCodeAsync(string joinCode, int userId)
        {
            var activity = await _context.TimelineActivities
                .FirstOrDefaultAsync(a => a.JoinCode == joinCode);

            if (activity == null)
                return false;

            // Sprawdź czy już jest uczestnikiem
            var alreadyMember = await _context.ActivityMembers
                .AnyAsync(am => am.ActivityId == activity.ActivityId && am.UserId == userId);

            if (alreadyMember)
                return false;

            // Dodaj użytkownika jako uczestnika (auto accepted)
            var newMember = new ActivityMembers
            {
                ActivityId = activity.ActivityId,
                UserId = userId,
                Role = "participant",
                Status = "auto_accepted"
            };

            _context.ActivityMembers.Add(newMember);
            await _context.SaveChangesAsync();

            return true;
        }

        //PATCH akceptacja zaproszenia
        //public async Task<bool> AcceptInviteAsync(int activityId, int userId)
        //{
        //    var activityMember = await _context.ActivityMembers
        //        .FirstOrDefaultAsync(am => am.ActivityId == activityId && am.UserId == userId && am.Status == "pending");

        //    if (activityMember == null) return false; // zaproszenie nie istnieje lub już zostało zaakceptowane

        //    activityMember.Status = "accepted"; // zmieniamy status na "accepted"
        //    await _context.SaveChangesAsync();

        //    return true;
        //}

        public async Task<bool> UpdateInviteStatusAsync(int activityId, int userId, string status)
        {
            // znajdź zaproszenie dla użytkownika
            var invite = await _context.ActivityMembers
                .FirstOrDefaultAsync(am => am.ActivityId == activityId && am.UserId == userId);

            if (invite == null)
                return false; // brak zaproszenia

            // sprawdź poprawność statusu
            if (status != "accepted" && status != "declined")
                return false;

            invite.Status = status;
            await _context.SaveChangesAsync();

            return true;
        }


        //DELETE usuniecie (cofniecie zaproszenia)
        //public async Task<bool> RevokeInviteAsync(int activityId, int userId)
        //{
        //    var activityMember = await _context.ActivityMembers
        //        .FirstOrDefaultAsync(am => am.ActivityId == activityId && am.UserId == userId && am.Status == "pending");

        //    if (activityMember == null) return false; // zaproszenie nie istnieje lub już zostało zaakceptowane

        //    _context.ActivityMembers.Remove(activityMember);
        //    await _context.SaveChangesAsync();

        //    return true;
        //}

        //DELETE usuniecie (cofniecie zaproszenia)
        public async Task<bool> RevokeInviteAsync(int activityId, int targetUserId, int ownerId)
        {
            // Pobierz aktywność wraz z ownerem
            var activity = await _context.TimelineActivities
                .FirstOrDefaultAsync(a => a.ActivityId == activityId && a.OwnerId == ownerId);

            if (activity == null) return false; // nie istnieje lub nie jesteś ownerem

            // Znajdź zaproszenie w statusie pending
            var invite = await _context.ActivityMembers
                .FirstOrDefaultAsync(am => am.ActivityId == activityId
                                           && am.UserId == targetUserId
                                           && am.Status == "pending");

            if (invite == null) return false; // brak zaproszenia pending

            _context.ActivityMembers.Remove(invite);
            await _context.SaveChangesAsync();

            return true;
        }
        //DELETE usuniecie istniejacego uczestnika
        public async Task<bool> RemoveParticipantAsync(int activityId, int targetUserId, int ownerId)
        {
            // Pobierz aktywność wraz z ownerem
            var activity = await _context.TimelineActivities
                .FirstOrDefaultAsync(a => a.ActivityId == activityId && a.OwnerId == ownerId);

            if (activity == null) return false; // nie jesteś ownerem

            // Znajdź uczestnika w statusie accepted
            var participant = await _context.ActivityMembers
                .FirstOrDefaultAsync(am => am.ActivityId == activityId
                                           && am.UserId == targetUserId
                                           && am.Role == "participant"
                                           && am.Status == "accepted");

            if (participant == null) return false; // brak uczestnika

            _context.ActivityMembers.Remove(participant);
            await _context.SaveChangesAsync();

            return true;
        }

        //DELETE usuniecie swojego uczestnictwa (tylko uzytkownik)
        public async Task<bool> CancelInviteAsync(int activityId, int userId)
        {
            var member = await _context.ActivityMembers
                .FirstOrDefaultAsync(am => am.ActivityId == activityId && am.UserId == userId && am.Role == "participant");

            if (member == null) return false;

            _context.ActivityMembers.Remove(member);
            await _context.SaveChangesAsync();

            return true;
        }

        //DELETE usuwa wszystkich z aktywnosci
        public async Task<bool> CancelActivityAsync(int activityId, int ownerId)
        {
            var activity = await _context.TimelineActivities
                .Include(a => a.ActivityMembers)
                .FirstOrDefaultAsync(a => a.ActivityId == activityId && a.OwnerId == ownerId);

            if (activity == null) return false;

            _context.ActivityMembers.RemoveRange(activity.ActivityMembers);

            await _context.SaveChangesAsync();
            return true;

        }
    }
}
