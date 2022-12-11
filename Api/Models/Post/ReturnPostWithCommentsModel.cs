using Api.Models.Attachment;
using Api.Models.PostComment;
using Api.Models.User;

namespace Api.Models.Post
{
    public class ReturnPostWithCommentsModel
    {
        public Guid Id { get; set; }
        public string? Caption { get; set; }
        public PostUserModel Author { get; set; } = null!;
        public List<AttachmentExternalModel> PostAttachments { get; set; } = null!;
        public List<ReturnPostCommentModel>? PostComments { get; set; }
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }
    }
}