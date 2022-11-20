using Api.Models.Post;

namespace Api.Models.PostComment
{
    public class PostCommentModel
    {
        public Guid AuthorId { get; set; }
        public Guid PostId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string? Caption { get; set; }
        public List<PostCommentAttachmentModel>? PostCommentAttachments { get; set; }
    }
}
