namespace Api.Models.Like
{
    public class PostCommentLikeModel
    {
        public Guid? AuthorId { get; set; }
        public Guid PostCommentId { get; set; }
        public bool IsPositive { get; set; }
    }
}
