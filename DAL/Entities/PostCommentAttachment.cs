namespace DAL.Entities
{
    public class PostCommentAttachment : Attachment
    {
        public virtual PostComment PostComment { get; set; } = null!;
    }
}
