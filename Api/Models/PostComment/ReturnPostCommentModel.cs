using Api.Models.Attachment;
using Api.Models.User;

namespace Api.Models.PostComment
{
    public class ReturnPostCommentModel
    {
        public Guid Id { get; set; }
        public string? Caption { get; set; }
        public string CreatedDate { get; set; } = null!;
        public PostUserModel Author { get; set; } = null!;
        public List<AttachmentExternalModel>? PostCommentAttachments { get; set; }
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }
    }
}
