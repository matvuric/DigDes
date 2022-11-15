﻿using Api.Models.Attachment;
using Api.Services;
using Common.Consts;
using Common.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AttachmentController : ControllerBase
    {
        private readonly AttachmentService _attachmentService;
        private readonly UserService _userService;
        private readonly PostService _postService;

        public AttachmentController(AttachmentService attachmentService, UserService userService, PostService postService)
        {
            _attachmentService = attachmentService;
            _userService = userService;
            _postService = postService;
        }

        [HttpPost]
        public async Task<List<MetadataModel>> UploadFiles([FromForm] List<IFormFile> files)
        {
            return await _attachmentService.UploadFiles(files);
        }

        [HttpGet]
        public async Task<FileStreamResult> GetCurrentUserAvatar(bool download = false)
        {
            return await GetUserAvatar(User.GetClaimValue<Guid>(ClaimNames.Id), download);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<FileStreamResult> GetUserAvatar(Guid userId, bool download = false)
        {
            return RenderAttachment(await _userService.GetAvatarById(userId), download);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<FileStreamResult> GetPostAttachment(Guid postAttachmentId, bool download = false)
        {
            return RenderAttachment(await _postService.GetPostAttachmentById(postAttachmentId), download);
        } 

        private FileStreamResult RenderAttachment(AttachmentModel attachment, bool download)
        {
            if (attachment == null)
            {
                throw new Exception("Attachment not found");
            }
            else
            {
                var fileStream = new FileStream(attachment.FilePath, FileMode.Open);

                if (download)
                {
                    return File(fileStream, attachment.MimeType, attachment.Name);
                }
                else
                {
                    return File(fileStream, attachment.MimeType);
                }
            }
        }
    }
}