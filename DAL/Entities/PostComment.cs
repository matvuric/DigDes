namespace DAL.Entities
{
    public class PostComment
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }

        public virtual Post Post { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
