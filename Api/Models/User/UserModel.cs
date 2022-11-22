namespace Api.Models.User
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public int PostsCount { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingsCount { get; set; }
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }
    }

    public class UserAvatarModel : UserModel
    {
        public string? AvatarLink { get; set; }
    }
}
