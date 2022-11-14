namespace DAL.Entities
{
    public class Avatar: Attachment
    {
        public Guid OwnerId { get; set; }
        public virtual User? Owner { get; set; }
    }
}
