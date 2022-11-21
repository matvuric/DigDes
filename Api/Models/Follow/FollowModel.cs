namespace Api.Models.Follow
{
    public class FollowModel
    {
        public Guid? FollowerId { get; set; }
        public Guid FollowingId { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
