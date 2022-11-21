namespace DAL.Entities
{
    public class PostCommentLike : Like
    {
        public Guid PostCommentId { get; set; }

        public virtual PostComment PostComment { get; set; } = null!;
    }
}
