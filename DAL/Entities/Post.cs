namespace DAL.Entities
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public DateTimeOffset CreatedDate { get; set; }
        public Guid AuthorId { get; set; }

        public virtual User Author { get; set; } = null!;
        public virtual ICollection<PostAttach> Attaches { get; set; } = null!;
        public virtual ICollection<PostComment> Comments { get; set; } = null!;
    }
}
