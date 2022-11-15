using Api.Models.Attachment;

namespace Api.Models.Post
{
    public class CreatePostModel
    {
        public string? Caption { get; set; }
        public List<MetadataModel>? Attachments { get; set; }
    }
}
