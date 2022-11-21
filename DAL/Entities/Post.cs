namespace DAL.Entities
{
    public class Post
    {
        public Guid Id { get; set; }
        public string? Caption { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public Guid AuthorId { get; set; }

        public virtual User Author { get; set; } = null!;
        public virtual ICollection<PostAttachment>? PostAttachments { get; set; }
        public virtual ICollection<PostComment>? Comments { get; set; }
        public virtual ICollection<PostLike>? Likes { get; set; }
    }
}
