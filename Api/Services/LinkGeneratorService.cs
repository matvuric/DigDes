using Api.Models.Attachment;
using Api.Models.User;
using DAL.Entities;

namespace Api.Services
{
    public class LinkGeneratorService
    {
        public Func<Guid, string?>? LinkAvatarGenerator;
        public Func<Guid, string?>? LinkAttachmentGenerator;

        public void FixAvatar(User src, UserAvatarModel dest)
        {
            dest.AvatarLink = src.Avatar == null ? null : LinkAvatarGenerator?.Invoke(src.Id);
        }

        public void FixProfile(User src, UserProfileModel dest)
        {
            dest.AvatarLink = src.Avatar == null ? null : LinkAvatarGenerator?.Invoke(src.Id);
        }

        public void FixPostUserAvatar(User src, PostUserModel dest)
        {
            dest.AvatarLink = src.Avatar == null ? null : LinkAvatarGenerator?.Invoke(src.Id);
        }

        public void FixPostAttachment(PostAttachment src, AttachmentExternalModel dest)
        {
            dest.AttachmentLink = LinkAttachmentGenerator?.Invoke(src.Id);
        }

        public void FixPostCommentAttachment(PostCommentAttachment src, AttachmentExternalModel dest)
        {
            dest.AttachmentLink = LinkAttachmentGenerator?.Invoke(src.Id);
        }
    }
}
