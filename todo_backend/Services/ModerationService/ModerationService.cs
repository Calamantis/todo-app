using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using todo_backend.Data;
using todo_backend.Dtos.ModerationDto;
using todo_backend.Models;
using todo_backend.Services.AuditLogService;

namespace todo_backend.Services.ModerationService
{
    public class ModerationService : IModerationService
    {
        private readonly AppDbContext _context;
        private readonly IAuditLogService _logger;

        private const string DefaultProfileImagePath = "/UserProfileImages/DefaultProfileImage.jpg";
        private const string DefaultBackgroundImagePath = "/UserProfileImages/DefaultBgImage.jpg";

        public ModerationService(AppDbContext context, IAuditLogService logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ModeratedUserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();

            return users.Select(u => new ModeratedUserDto
            {
                UserId = u.UserId,
                Email = u.Email,
                DisplayName = u.FullName,
                Description = u.Synopsis,
                ProfileImageUrl = u.ProfileImageUrl,
                BackgroundImageUrl = u.BackgroundImageUrl,
                Role = u.Role.ToString()
            });
        }

        public async Task UpdateUserProfileImageAsync(int moderatorId, int userId, string url)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            user.ProfileImageUrl = url;
            await _context.SaveChangesAsync();
            await _logger.LogAsync(moderatorId, "UPDATE_PROFILE_IMAGE", "User", userId, "Updated users profile image");
        }

        public async Task ResetUserProfileImageAsync(int moderatorId, int userId)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            user.ProfileImageUrl = DefaultProfileImagePath;
            await _context.SaveChangesAsync();
            await _logger.LogAsync(moderatorId, "DELETE_PROFILE_IMAGE", "User", userId, "Reset users profile image");
        }

        public async Task UpdateUserBackgroundImageAsync(int moderatorId, int userId, string url)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            user.BackgroundImageUrl = url;
            await _context.SaveChangesAsync();
            await _logger.LogAsync(moderatorId, "UPDATE_BG_IMAGE", "User", userId, "Updated users background image");
        }

        public async Task ResetUserBackgroundImageAsync(int moderatorId, int userId)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            user.BackgroundImageUrl = DefaultBackgroundImagePath;
            await _context.SaveChangesAsync();
            await _logger.LogAsync(moderatorId, "DELETE_BG_IMAGE", "User", userId, "Reset users background image");
        }

        public async Task UpdateUserDisplayNameAsync(int moderatorId, int userId, string value)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            user.FullName = value;
            await _context.SaveChangesAsync();
            await _logger.LogAsync(moderatorId, "UPDATE_NAME", "User", userId, "Updated users display name");
        }

        public async Task ResetUserDisplayNameAsync(int moderatorId, int userId)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            user.FullName = $"User{user.UserId}";
            await _context.SaveChangesAsync();
            await _logger.LogAsync(moderatorId, "DELETE_NAME", "User", userId, "Reset users display name");
        }

        public async Task UpdateUserDescriptionAsync(int moderatorId, int userId, string value)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            user.Synopsis = value;
            await _context.SaveChangesAsync();
            await _logger.LogAsync(moderatorId, "DELETE_DESCRIPTION", "User", userId, "Updated users description");
        }

        public async Task ResetUserDescriptionAsync(int moderatorId, int userId)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            user.Synopsis = null;
            await _context.SaveChangesAsync();
            await _logger.LogAsync(moderatorId, "DELETE_DESCRIPTION", "User", userId, "Reset users description");
        }

        //public async Task<IEnumerable<ModeratedActivityDto>> GetAllActivitiesAsync()
        //{
        //    var activities = await _context.Activities.ToListAsync();

        //    return activities.Select(a => new ModeratedActivityDto
        //    {
        //        ActivityId = a.ActivityId,
        //        OwnerId = a.OwnerId,
        //        Title = a.Title,
        //        Description = a.Description,
        //        IsRecurring = a.IsRecurring
        //    });
        //}

        public async Task<IEnumerable<ModeratedActivityDto>> GetAllActivitiesAsync()
        {
            return await (
                from a in _context.Activities
                join u in _context.Users on a.OwnerId equals u.UserId
                select new
                {
                    Activity = a,
                    OwnerEmail = u.Email,
                    Instances = _context.ActivityInstances.Count(i => i.ActivityId == a.ActivityId)
                }
            )
            .Select(x => new ModeratedActivityDto
            {
                ActivityId = x.Activity.ActivityId,
                OwnerId = x.Activity.OwnerId,
                OwnerEmail = x.OwnerEmail,

                Title = x.Activity.Title,
                Description = x.Activity.Description,
                IsRecurring = x.Activity.IsRecurring,

                IsOnline = x.Activity.JoinCode != null || x.Activity.isFriendsOnly, // NEW
                InstancesCount = x.Instances                                        // NEW
            })
            .ToListAsync();
        }

        public async Task UpdateActivityTitleAsync(int moderatorId, int activityId, string value)
        {
            var activity = await _context.Activities.FindAsync(activityId)
                ?? throw new KeyNotFoundException("Activity not found.");

            activity.Title = value;
            await _context.SaveChangesAsync();
            await _logger.LogAsync(moderatorId, "UPDATE_TITLE", "Activity", activityId, "Updated activity title");
        }

        public async Task ResetActivityTitleAsync(int moderatorId, int activityId)
        {
            var activity = await _context.Activities.FindAsync(activityId)
                ?? throw new KeyNotFoundException("Activity not found.");

            activity.Title = $"Activity {activity.ActivityId}";
            await _context.SaveChangesAsync();
            await _logger.LogAsync(moderatorId, "RESET_TITLE", "Activity", activityId, "Reset activity title");
        }

        public async Task UpdateActivityDescriptionAsync(int moderatorId, int activityId, string value)
        {
            var activity = await _context.Activities.FindAsync(activityId)
                ?? throw new KeyNotFoundException("Activity not found.");

            activity.Description = value;
            await _context.SaveChangesAsync();
            await _logger.LogAsync(moderatorId, "UPDATE_DESCRIPTION", "Activity", activityId, "Updated activity description");
        }

        public async Task ResetActivityDescriptionAsync(int moderatorId, int activityId)
        {
            var activity = await _context.Activities.FindAsync(activityId)
                ?? throw new KeyNotFoundException("Activity not found.");

            activity.Description = null;
            await _context.SaveChangesAsync();
            await _logger.LogAsync(moderatorId, "UPDATE_DESCRIPTION", "Activity", activityId, "Updated activity description");
        }

    }
}
