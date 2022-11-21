using System.ComponentModel.DataAnnotations;

namespace Api.Models.User
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Bio { get; set; } = null!;
        public DateTimeOffset BirthDate { get; set; }
        public int PostsCount { get; set; }
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }
    }

    public class UserAvatarModel : UserModel
    {
        public string? AvatarLink { get; set; }
    }
}
