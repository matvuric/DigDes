﻿using Api.Models.Attachment;
using Api.Models.User;

namespace Api.Models.PostComment
{
    public class ReturnPostCommentModel
    {
        public Guid Id { get; set; }
        public string? Caption { get; set; }
        public UserAvatarModel Author { get; set; } = null!;
        public List<AttachmentExternalModel>? PostCommentAttachments { get; set; }
    }
}