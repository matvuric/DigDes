namespace Api.Models.User
{
    public class PostUserModel
    {
        public Guid Id { get; set; }
        public string? AvatarLink { get; set; }
        public string Username { get; set; } = null!;
        public int FollowersCount { get; set; }
        public int FollowingsCount { get; set; }
    }
}
