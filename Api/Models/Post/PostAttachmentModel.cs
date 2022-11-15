namespace Api.Models.Post
{
    public class PostAttachmentModel
    {
        public Guid TempId { get; set; }
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public long Size { get; set; }
        public string FilePath { get; set; } = null!;
        public Guid AuthorId { get; set; }
    }
}
