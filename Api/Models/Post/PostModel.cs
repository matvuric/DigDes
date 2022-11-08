namespace Api.Models.Post
{
    public class PostModel
    {
        public Guid UserId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string Text { get; set; } = null!;
        public List<PostAttachModel> Attaches { get; set; } = null!;
    }
}
