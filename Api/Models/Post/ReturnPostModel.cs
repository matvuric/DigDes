﻿using Api.Models.Attachment;
using Api.Models.User;

namespace Api.Models.Post
{
    public class ReturnPostModel
    {
        public Guid Id { get; set; }
        public string? Caption { get; set; }
        public UserAvatarModel Author { get; set; } = null!;
        public List<AttachmentExternalModel>? PostAttachments { get; set; }
    }
}