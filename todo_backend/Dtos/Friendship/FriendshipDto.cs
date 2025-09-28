namespace todo_backend.Dtos.Friendship
{
    public class FriendshipDto
    {
        public DateTime FriendsSince { get; set; }

        //uproszczone dane znajomego 
        public string FriendFullName { get; set; } = string.Empty;
        public string FriendEmail { get; set; } = string.Empty;

    }
}
