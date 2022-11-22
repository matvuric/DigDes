using Api.Models.Attachment;

namespace Api.Models.PostComment
{
    public class PostCommentModel
    {
        public Guid AuthorId { get; set; }
        public Guid PostId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string? Caption { get; set; }
        public List<MetadataLinkModel>? PostCommentAttachments { get; set; }
    }
}
