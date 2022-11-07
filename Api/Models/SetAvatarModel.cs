namespace Api.Models
{
    public class SetAvatarModel
    {
        public MetaDataModel Avatar { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}
