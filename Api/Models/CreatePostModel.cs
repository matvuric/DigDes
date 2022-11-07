namespace Api.Models
{
    public class CreatePostModel
    {
        public string Text { get; set; } = null!;
        public Guid UserId { get; set; }
        public ICollection<MetaDataModel> Attaches { get; set; } = null!;
    }
}
