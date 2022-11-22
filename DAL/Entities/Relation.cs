namespace DAL.Entities
{
    public class Relation
    {
        public Guid FollowerId { get; set; }
        public Guid FollowingId { get; set; }
        public DateTimeOffset FollowDate { get; set; }
        public bool IsConfirmed { get; set; }

        public virtual User FollowerUser { get; set; } = null!;
        public virtual User FollowingUser { get; set; } = null!;
    }
}
