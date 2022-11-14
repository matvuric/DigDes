namespace DAL.Entities
{
    public class PostAttachment : Attachment
    {
        public virtual Post Post { get; set; } = null!;
    }
}
