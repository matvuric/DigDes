namespace DAL.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTimeOffset BirthDate { get; set; }

        public virtual Avatar? Avatar { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }
        public virtual ICollection<UserSession>? Sessions { get; set; }

    }
}
