namespace Api.Models.User
{
    public class UserProfileModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Bio { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTimeOffset BirthDate { get; set; }
        public bool IsPrivate { get; set; } = false;
        public int PostsCount { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingsCount { get; set; }
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }
        public string? AvatarLink { get; set; }
    }
}
