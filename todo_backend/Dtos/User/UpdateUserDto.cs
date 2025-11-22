using System.ComponentModel.DataAnnotations;

namespace todo_backend.Dtos.User
{
    public class UpdateUserDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string? ProfileImageUrl { get; set; }
        public string? BackgroundImageUrl { get; set; }
        public string? Synopsis { get; set; }

        public IFormFile? ProfileImage { get; set; }
        public IFormFile? BackgroundImage { get; set; }

        public bool AllowFriendInvites { get; set; } = true;

        public bool AllowDataStatistics { get; set; } = true;
    }
}
