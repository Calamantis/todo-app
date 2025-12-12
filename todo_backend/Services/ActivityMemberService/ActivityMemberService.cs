using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using todo_backend.Data;
using todo_backend.Dtos.ActivityMemberDto;
using todo_backend.Models;

namespace todo_backend.Services.ActivityMembersService
{
    public class ActivityMemberService : IActivityMemberService
    {
        private readonly AppDbContext _context;

        public ActivityMemberService(AppDbContext context)
        {
            _context = context;
        }

        //GET instancje online ktorych jestesmy uczestnikami
        public async Task<IEnumerable<ActivityInviteDto>> GetOnlineActivitiesAsync(int userId)
        {
            return await _context.ActivityMembers.Where(am => am.UserId == userId && am.Role == "participant" && am.Status == "accepted")
                .Include(am => am.Activity)
                .Select(am => new ActivityInviteDto
                {
                    ActivityId = am.ActivityId,
                    ActivityTitle = am.Activity.Title,
                    InvitedUserId = am.Activity.OwnerId,
                    FullName = am.Activity.Owner.FullName,
                    Email = am.Activity.Owner.Email,
                    ProfileImage = am.Activity.Owner.ProfileImageUrl,
                    BackgroundImage = am.Activity.Owner.ProfileImageUrl,
                    Status = am.Status,
                    Role = "participant"
                })
                .ToListAsync();
        }


        ////GET przeglądaj dostane zaproszenia
        //public async Task<IEnumerable<ActivityInviteDto?>> GetSentInvitesAsync(int userId)
        //{
        //    var sentInvites = await _context.ActivityMembers
        //                   .Where(am => am.UserId == userId && am.Status == "pending")
        //                   .Include(am => am.Activity)
        //                   .ThenInclude(a => a.Owner)
        //                   .Select(am => new ActivityInviteDto
        //                   {
        //                       ActivityId = am.ActivityId,
        //                       ActivityTitle = am.Activity.Title,
        //                       OwnerFullName = am.Activity.Owner.FullName,
        //                       Status = am.Status
        //                   })
        //                   .ToListAsync();

        //    return sentInvites;
        //}

        public async Task<IEnumerable<ActivityInviteDto>> GetReceivedInvitesAsync(int userId)
        {
            return await _context.ActivityMembers
                .Where(am =>
                    am.UserId == userId &&
                    am.Status == "pending")
                .Include(am => am.Activity)
                .ThenInclude(a => a.Owner)
                .Select(am => new ActivityInviteDto
                {
                    ActivityId = am.ActivityId,
                    ActivityTitle = am.Activity.Title,
                    InvitedUserId = am.Activity.OwnerId,
                    FullName = am.Activity.Owner.FullName,
                    Email = am.Activity.Owner.Email,
                    ProfileImage = am.Activity.Owner.ProfileImageUrl,
                    BackgroundImage = am.Activity.Owner.ProfileImageUrl,
                    Status = am.Status,
                    Role = "participant"
                })
                .ToListAsync();
        }

        ////GET przeglądaj zaakceptowane zaproszenia
        //public async Task<IEnumerable<ActivityInviteDto?>> GetAcceptedInvitesAsync(int userId)
        //{
        //    var sentInvites = await _context.ActivityMembers
        //                   .Where(am => am.UserId == userId && am.Status == "accepted")
        //                   .Include(am => am.Activity)
        //                   .ThenInclude(a => a.Owner)
        //                   .Select(am => new ActivityInviteDto
        //                   {
        //                       ActivityId = am.ActivityId,
        //                       ActivityTitle = am.Activity.Title,
        //                       OwnerFullName = am.Activity.Owner.FullName,
        //                       Status = am.Status
        //                   })
        //                   .ToListAsync();


        //    return sentInvites;
        //}

        public async Task<IEnumerable<ActivityInviteDto>> GetAcceptedMembersAsync(int activityId, int userId)
        {
            return await _context.ActivityMembers
                .Where(am =>
                    am.ActivityId == activityId &&
                    am.Status == "accepted" &&
                    am.Role == "participant")
                    
                .Include(am => am.User)
                .Select(am => new ActivityInviteDto
                {
                    ActivityId = am.ActivityId,
                    ActivityTitle = am.Activity.Title,
                    InvitedUserId = am.UserId,
                    FullName = am.User.FullName,
                    Email = am.User.Email,
                    ProfileImage = am.User.ProfileImageUrl,
                    BackgroundImage = am.User.BackgroundImageUrl,
                    Status = am.Status,
                    Role = am.Role
                })
                .ToListAsync();
        }


        ////GET przeglądaj wysłane zaproszenia
        //public async Task<IEnumerable<FullActivityMembersDto?>> GetSentInvitesAsync(int activityId, int requestingUserId)
        //{
        //    var isParticipantOrOwner = await _context.TimelineActivities
        //        .AnyAsync(a => a.ActivityId == activityId && a.OwnerId == requestingUserId)
        //        ||
        //         await _context.ActivityMembers
        //         .AnyAsync(am => am.ActivityId == activityId && am.UserId == requestingUserId);

        //    if (!isParticipantOrOwner)
        //        return null; // lub throw new UnauthorizedAccessException();


        //    var participants = await _context.ActivityMembers
        //                    .Where(am => am.ActivityId == activityId && am.Status == "pending")
        //                    .Include(am => am.User)
        //                    .Select(am => new FullActivityMembersDto
        //                    {
        //                        ActivityId = am.ActivityId,
        //                        UserId = am.UserId,
        //                        Role = am.Role,
        //                        Status = am.Status,
        //                        UserFullName = am.User.FullName,
        //                        UserEmail = am.User.Email
        //                    })
        //                    .ToListAsync();

        //    return participants;
        //}

        //GET przeglądaj wysłane zaproszenia
        public async Task<IEnumerable<ActivityInviteDto>> GetSentInvitesAsync(int activityId, int ownerId)
        {
            return await _context.ActivityMembers
                .Where(am => am.Activity.OwnerId == ownerId && am.ActivityId == activityId && am.Role == "participant" && am.Status == "pending")
                .Include(am => am.User)
                .Include(am => am.Activity)
                .Select(am => new ActivityInviteDto
                {
                    ActivityId = am.ActivityId,
                    ActivityTitle = am.Activity.Title,
                    InvitedUserId = am.UserId,
                    FullName = am.User.FullName,
                    Email = am.User.Email,
                    ProfileImage = am.User.ProfileImageUrl,
                    BackgroundImage = am.User.BackgroundImageUrl,
                    Status = am.Status,
                    Role = am.Role
                })
                .ToListAsync();
        }

        ////GET zaakceptowane zaproszenia
        //public async Task<IEnumerable<FullActivityMembersDto?>> GetParticipantsOfActivityAsync(int activityId, int requestingUserId)
        //{
        //    var isParticipantOrOwner = await _context.TimelineActivities
        //        .AnyAsync(a => a.ActivityId == activityId && a.OwnerId == requestingUserId)
        //         ||
        //         await _context.ActivityMembers
        //           .AnyAsync(am => am.ActivityId == activityId && am.UserId == requestingUserId);

        //    if (!isParticipantOrOwner)
        //        return null; // lub throw new UnauthorizedAccessException();


        //    var participants = await _context.ActivityMembers
        //        .Where(am => am.ActivityId == activityId && am.Status == "accepted" || am.Status == "auto_accepted")
        //        .Include(am => am.User)
        //        .Select(am => new FullActivityMembersDto
        //        {
        //            ActivityId = am.ActivityId,
        //            UserId = am.UserId,
        //            Role = am.Role,
        //            Status = am.Status,
        //            UserFullName = am.User.FullName,
        //            UserEmail = am.User.Email
        //        })
        //        .ToListAsync();

        //    return participants;
        //}

        ////POST wyslanie zaproszenia
        //public async Task<ActivityInviteDto?> SendInviteAsync(int activityId, int ownerId, int invitedUserId)
        //{
        //    // Nie można zaprosić samego siebie
        //    if (ownerId == invitedUserId) return null;

        //    var activity = await _context.TimelineActivities
        //        .Include(a => a.User) // wczytanie ownera
        //        .FirstOrDefaultAsync(a => a.ActivityId == activityId);
        //    if (activity == null || activity.OwnerId != ownerId) return null;

        //    // Sprawdź czy osoba zapraszaja jest w znajomych
        //    var isFriend = await _context.Friendships
        //        .AnyAsync(f =>
        //        ((f.UserId == ownerId && f.FriendId == invitedUserId) ||
        //        (f.UserId == invitedUserId && f.FriendId == ownerId)) &&
        //        f.Status == "accepted");
        //    if (!isFriend) return null; // jeśli nie jest znajomym, zwracamy false

        //    // Sprawdź czy taki rekord już istnieje
        //    var exists = await _context.ActivityMembers
        //        .AnyAsync(am => am.ActivityId == activityId && am.UserId == invitedUserId);
        //    if (exists) return null;

        //    var activityMember = new ActivityMembers
        //    {
        //        ActivityId = activityId,
        //        UserId = invitedUserId,
        //        Role = "participant",
        //        Status = "pending"
        //    };

        //    _context.ActivityMembers.Add(activityMember);
        //    await _context.SaveChangesAsync();

        //    return new ActivityInviteDto
        //    {
        //        ActivityId = activity.ActivityId,
        //        InvitedUserId = invitedUserId,
        //        ActivityTitle = activity.Title,
        //        OwnerFullName = activity.User.FullName,
        //        Status = activityMember.Status
        //    };
        //}

        public async Task<bool> SendInviteAsync(int ownerId, int activityId, int friendId)
        {
            if (ownerId == friendId) return false;

            // sprawdź czy owner -> friend jest znajomym
            bool areFriends = await _context.Friendships
                .AnyAsync(f =>
                    f.Status == "accepted" &&
                    (
                        (f.UserId == ownerId && f.FriendId == friendId) ||
                        (f.UserId == friendId && f.FriendId == ownerId)
                    )
                );

            if (!areFriends) return false;

            // sprawdź czy nie jest już członkiem
            bool exists = await _context.ActivityMembers
                .AnyAsync(am => am.ActivityId == activityId && am.UserId == friendId);

            if (exists) return false;

            var invite = new ActivityMember
            {
                ActivityId = activityId,
                UserId = friendId,
                Status = "pending",
                Role = "participant"
            };

            _context.ActivityMembers.Add(invite);
            await _context.SaveChangesAsync();
            return true;
        }


        ////POST dolaczenie po kodzie
        //public async Task<bool> JoinActivityByCodeAsync(string joinCode, int userId)
        //{
        //    var activity = await _context.TimelineActivities
        //        .FirstOrDefaultAsync(a => a.JoinCode == joinCode);

        //    if (activity == null)
        //        return false;

        //    // Sprawdź czy już jest uczestnikiem
        //    var alreadyMember = await _context.ActivityMembers
        //        .AnyAsync(am => am.ActivityId == activity.ActivityId && am.UserId == userId);

        //    if (alreadyMember)
        //        return false;

        //    // Dodaj użytkownika jako uczestnika (accepted)
        //    var newMember = new ActivityMembers
        //    {
        //        ActivityId = activity.ActivityId,
        //        UserId = userId,
        //        Role = "participant",
        //        Status = "accepted"
        //    };

        //    _context.ActivityMembers.Add(newMember);
        //    await _context.SaveChangesAsync();

        //    return true;
        //}

        public async Task<bool> JoinByCodeAsync(int userId, string joinCode)
        {
            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.JoinCode == joinCode);

            if (activity == null)
                return false;

            // sprawdź blokadę
            var blocked = await _context.ActivityMembers
                .AnyAsync(am => am.ActivityId == activity.ActivityId && am.UserId == userId && am.Status == "blocked");

            if (blocked)
                return false;

            // sprawdź duplikat
            bool exists = await _context.ActivityMembers
                .AnyAsync(am => am.ActivityId == activity.ActivityId && am.UserId == userId);

            if (!exists)
            {
                _context.ActivityMembers.Add(new ActivityMember
                {
                    ActivityId = activity.ActivityId,
                    UserId = userId,
                    Role = "participant",
                    Status = "accepted"
                });

                await _context.SaveChangesAsync();
            }

            return true;
        }

        //public async Task<bool> UpdateInviteStatusAsync(int activityId, int userId, string status)
        //{
        //    // znajdź zaproszenie dla użytkownika
        //    var invite = await _context.ActivityMembers
        //        .FirstOrDefaultAsync(am => am.ActivityId == activityId && am.UserId == userId);

        //    if (invite == null)
        //        return false; // brak zaproszenia

        //    // sprawdź poprawność statusu
        //    if (status != "accepted" && status != "declined")
        //        return false;

        //    invite.Status = status;
        //    await _context.SaveChangesAsync();

        //    return true;
        //}

        public async Task<bool> UpdateInviteStatusAsync(int userId, int activityId, string newStatus)
        {
            var member = await _context.ActivityMembers
                .FirstOrDefaultAsync(am => am.ActivityId == activityId && am.UserId == userId);

            if (member == null)
                return false;

            member.Status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        ////DELETE usuniecie (cofniecie zaproszenia)
        //public async Task<bool> RevokeInviteAsync(int activityId, int targetUserId, int ownerId)
        //{
        //    // Pobierz aktywność wraz z ownerem
        //    var activity = await _context.TimelineActivities
        //        .FirstOrDefaultAsync(a => a.ActivityId == activityId && a.OwnerId == ownerId);

        //    if (activity == null) return false; // nie istnieje lub nie jesteś ownerem

        //    // Znajdź zaproszenie w statusie pending
        //    var invite = await _context.ActivityMembers
        //        .FirstOrDefaultAsync(am => am.ActivityId == activityId
        //                                   && am.UserId == targetUserId
        //                                   && am.Status == "pending");

        //    if (invite == null) return false; // brak zaproszenia pending

        //    _context.ActivityMembers.Remove(invite);
        //    await _context.SaveChangesAsync();

        //    return true;
        //}

        public async Task<bool> CancelInviteAsync(int ownerId, int activityId, int targetUserId)
        {
            var member = await _context.ActivityMembers
                .Include(am => am.Activity)
                .FirstOrDefaultAsync(am => am.ActivityId == activityId && am.UserId == targetUserId && am.Status == "pending");

            if (member == null || member.Activity.OwnerId != ownerId)
                return false;

            _context.ActivityMembers.Remove(member);
            await _context.SaveChangesAsync();
            return true;
        }

        ////DELETE usuniecie istniejacego uczestnika
        //public async Task<bool> RemoveParticipantAsync(int activityId, int targetUserId, int ownerId)
        //{
        //    // Pobierz aktywność wraz z ownerem
        //    var activity = await _context.TimelineActivities
        //        .FirstOrDefaultAsync(a => a.ActivityId == activityId && a.OwnerId == ownerId);

        //    if (activity == null) return false; // nie jesteś ownerem

        //    // Znajdź uczestnika w statusie accepted
        //    var participant = await _context.ActivityMembers
        //        .FirstOrDefaultAsync(am => am.ActivityId == activityId
        //                                   && am.UserId == targetUserId
        //                                   && am.Role == "participant"
        //                                   && am.Status == "accepted");

        //    if (participant == null) return false; // brak uczestnika

        //    _context.ActivityMembers.Remove(participant);
        //    await _context.SaveChangesAsync();

        //    return true;
        //}

        public async Task<bool> RemoveMemberAsync(int ownerId, int activityId, int targetUserId)
        {
            var member = await _context.ActivityMembers
                .Include(am => am.Activity)
                .FirstOrDefaultAsync(am => am.ActivityId == activityId && am.UserId == targetUserId && am.Status == "accepted");

            if (member == null || member.Activity.OwnerId != ownerId)
                return false;

            _context.ActivityMembers.Remove(member);
            await _context.SaveChangesAsync();
            return true;
        }

        ////DELETE usuniecie swojego uczestnictwa (tylko uzytkownik)
        //public async Task<bool> CancelInviteAsync(int activityId, int userId)
        //{
        //    var member = await _context.ActivityMembers
        //        .FirstOrDefaultAsync(am => am.ActivityId == activityId && am.UserId == userId && am.Role == "participant");

        //    if (member == null) return false;

        //    _context.ActivityMembers.Remove(member);
        //    await _context.SaveChangesAsync();

        //    return true;
        //}

        public async Task<bool> LeaveActivityAsync(int userId, int activityId)
        {
            var mem = await _context.ActivityMembers
                .FirstOrDefaultAsync(am => am.ActivityId == activityId && am.UserId == userId);

            if (mem == null)
                return false;

            _context.ActivityMembers.Remove(mem);
            await _context.SaveChangesAsync();
            return true;
        }

        //DELETE usun i zablokuj mozliwosc ponownego dolaczenia
        public async Task<bool> RemoveAndBlockAsync(int ownerId, int activityId, int targetUserId)
        {
            var member = await _context.ActivityMembers
                .Include(am => am.Activity)
                .FirstOrDefaultAsync(am => am.ActivityId == activityId && am.UserId == targetUserId);

            if (member == null || member.Activity.OwnerId != ownerId)
                return false;

            member.Status = "blocked";
            await _context.SaveChangesAsync();
            return true;
        }


        ////DELETE usuwa wszystkich z aktywnosci
        //public async Task<bool> CancelActivityAsync(int activityId, int ownerId)
        //{
        //    var activity = await _context.TimelineActivities
        //        .Include(a => a.ActivityMembers)
        //        .FirstOrDefaultAsync(a => a.ActivityId == activityId && a.OwnerId == ownerId);

        //    if (activity == null) return false;

        //    _context.ActivityMembers.RemoveRange(activity.ActivityMembers);

        //    await _context.SaveChangesAsync();
        //    return true;

        //}






    }
}
