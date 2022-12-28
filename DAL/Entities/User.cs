namespace DAL.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTimeOffset BirthDate { get; set; }
        public bool IsPrivate { get; set; }
        public string? PushToken { get; set; }

        public virtual Avatar? Avatar { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }
        public virtual ICollection<Like>? Likes { get; set; }
        public virtual ICollection<UserSession>? Sessions { get; set; }
        public virtual ICollection<Relation>? Followers { get; set; }
        public virtual ICollection<Relation>? Following { get; set; }
    }
}
