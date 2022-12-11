using Api.Models.Attachment;

namespace Api.Models.Post
{
    public class CreatePostModel
    {
        public Guid? AuthorId { get; set; }
        public string? Caption { get; set; }
        public List<MetadataModel> PostAttachments { get; set; } = null!;
    }
}
