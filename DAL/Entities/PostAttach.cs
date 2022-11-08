namespace DAL.Entities
{
    public class PostAttach : Attach
    {
        public virtual Post Post { get; set; } = null!;
    }
}
