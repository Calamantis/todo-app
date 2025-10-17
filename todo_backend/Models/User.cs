using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace todo_backend.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string FullName { get; set; } = string.Empty;

        public bool AllowMentions { get; set; } = true;

        public bool AllowFriendInvites { get; set; } = true;

        [MaxLength(255)]
        [Url(ErrorMessage = "Invalid profile image URL format.")]
        public string? ProfileImageUrl { get; set; } = "\\TempImageTests\\DefaultProfileImage.png";

        [MaxLength(255)]
        [Url(ErrorMessage = "Invalid background image URL format.")]
        public string? BackgroundImageUrl { get; set; } = "\\TempImageTests\\DefaultBgImage.jpg";

        [MaxLength(30)]
        public string? Synopsis { get; set; } = "No information.";

        [Required]
        public UserRole Role { get; set; } = UserRole.User;


        //opcjonalne, do latwego includowania w kodzie
        public ICollection<Friendship> Friendships { get; set; } = new List<Friendship>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<TimelineActivity> TimelineActivities { get; set; } = new List<TimelineActivity>();
        public ICollection<ActivityStorage> ActivityStorage { get; set; } = new List<ActivityStorage>();
        public ICollection<BlockedUsers> BlockedUsers { get; set; } = new List<BlockedUsers>();
        public ICollection<Statistics> Statistics { get; set; } = new List<Statistics>();
    }


    public enum UserRole
    {
        User,
        Moderator,
        Admin
    }

}