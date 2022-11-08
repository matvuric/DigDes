using Api.Models.Attach;

namespace Api.Models.Post
{
    public class CreatePostModel
    {
        public Guid UserId { get; set; }
        public string Text { get; set; } = null!;
        public DateTimeOffset CreatedDate { get; set; }
        public List<MetaDataModel> Attaches { get; set; } = null!;
    }
}
