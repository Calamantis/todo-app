using Microsoft.EntityFrameworkCore;
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

        //GET przeglądaj wysłane zaproszenia
        public async Task<IEnumerable<FullActivityMembersDto?>> GetSentInvitesAsync(int userId)
        {
            var sentInvites = await _context.ActivityMembers
                           .Where(am => am.UserId == userId && am.Status == "pending")
                           .Include(am => am.Activity)
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

            return sentInvites;
        }

        //GET przeglądaj zaakceptowane zaproszenia
        public async Task<IEnumerable<FullActivityMembersDto?>> GetAcceptedInvitesAsync(int userId)
        {
            var sentInvites = await _context.ActivityMembers
                           .Where(am => am.UserId == userId && am.Status == "accepted")
                           .Include(am => am.Activity)
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


            return sentInvites;
        }

        //POST wyslanie zaproszenia
        public async Task<bool> SendInviteAsync(int activityId, int ownerId, int invitedUserId)
        {
            // Nie można zaprosić samego siebie
            if (ownerId == invitedUserId) return false;

            var activity = await _context.TimelineActivities.FindAsync(activityId);
            if (activity == null || activity.OwnerId != ownerId) return false;

            // Sprawdź czy taki rekord już istnieje
            var exists = await _context.ActivityMembers
                .AnyAsync(am => am.ActivityId == activityId && am.UserId == invitedUserId);
            if (exists) return false;

            var activityMember = new ActivityMembers
            {
                ActivityId = activityId,
                UserId = invitedUserId,
                Role = "participant",
                Status = "pending"
            };

            _context.ActivityMembers.Add(activityMember);
            await _context.SaveChangesAsync();

            return true;
        }

        //PATCH akceptacja zaproszenia
        public async Task<bool> AcceptInviteAsync(int activityId, int userId)
        {
            var activityMember = await _context.ActivityMembers
                .FirstOrDefaultAsync(am => am.ActivityId == activityId && am.UserId == userId && am.Status == "pending");

            if (activityMember == null) return false; // zaproszenie nie istnieje lub już zostało zaakceptowane

            activityMember.Status = "accepted"; // zmieniamy status na "accepted"
            await _context.SaveChangesAsync();

            return true;
        }

        //DELETE usuniecie (cofniecie zaproszenia)
        public async Task<bool> RevokeInviteAsync(int activityId, int userId)
        {
            var activityMember = await _context.ActivityMembers
                .FirstOrDefaultAsync(am => am.ActivityId == activityId && am.UserId == userId && am.Status == "pending");

            if (activityMember == null) return false; // zaproszenie nie istnieje lub już zostało zaakceptowane

            _context.ActivityMembers.Remove(activityMember);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
