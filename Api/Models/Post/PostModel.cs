namespace Api.Models.Post
{
    public class PostModel
    {
        public Guid AuthorId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string? Caption { get; set; }
        public List<PostAttachmentModel>? PostAttachments { get; set; }
    }
}
