namespace todo_backend.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;


        //opcjonalne, do latwego includowania w kodzie

        //public ICollection<UserActivity> Activities { get; set; } = new List<UserActivity>();

        public ICollection<Friendship> Friendships { get; set; } = new List<Friendship>();
    }
}