namespace DAL.Entities
{
    public class PostComment
    {
        public Guid Id { get; set; }
        public string? Caption { get; set; } = null!;
        public DateTimeOffset CreatedDate { get; set; }
        public Guid AuthorId { get; set; }
        public Guid PostId { get; set; }

        public virtual Post Post { get; set; } = null!;
        public virtual User Author { get; set; } = null!;
        public virtual ICollection<PostCommentAttachment>? PostCommentAttachments { get; set; }
        public virtual ICollection<PostCommentLike>? Likes { get; set; }
    }
}
