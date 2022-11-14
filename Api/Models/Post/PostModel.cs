namespace Api.Models.Post
{
    public class PostModel
    {
        public Guid UserId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string Caption { get; set; } = null!;
        public List<PostAttachmentModel> PostAttachments { get; set; } = null!;
    }
}
