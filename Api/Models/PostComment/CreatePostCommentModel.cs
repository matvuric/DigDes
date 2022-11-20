using Api.Models.Attachment;

namespace Api.Models.PostComment
{
    public class CreatePostCommentModel
    {
        public Guid? AuthorId { get; set; }
        public Guid? PostId { get; set; }
        public string? Caption { get; set; }
        public List<MetadataModel>? PostCommentAttachments { get; set; }
    }
}
