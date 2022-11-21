namespace DAL.Entities
{
    public class Like
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public bool IsPositive { get; set; }

        public virtual User Author { get; set; } = null!;
    }
}
