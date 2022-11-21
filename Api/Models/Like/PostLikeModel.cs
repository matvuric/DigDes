namespace Api.Models.Like
{
    public class PostLikeModel
    {
        public Guid? AuthorId { get; set; }
        public Guid PostId { get; set; }
        public bool IsPositive { get; set; }
    }
}
