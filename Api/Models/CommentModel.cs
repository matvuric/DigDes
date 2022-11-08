namespace Api.Models
{
    public class CommentModel
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
    }
}
