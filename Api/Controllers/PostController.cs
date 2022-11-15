using Api.Models.Attachment;
using Api.Models.Post;
using Api.Models.User;
using Api.Services;
using Common.Consts;
using Common.Extentions;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;

        public PostController(PostService postService)
        {
            _postService = postService;

            if (postService != null)
            {
                _postService.SetLinkGenerator(user =>
                    Url.ControllerAction<AttachmentController>(nameof(AttachmentController.GetUserAvatar), new 
                        { 
                            userId = user.Id, 
                            download = false
                        }),
                    attachment => 
                        Url.ControllerAction<AttachmentController>(nameof(AttachmentController.GetPostAttachment), new
                        {
                            postAttachmentId = attachment.Id,
                            download = false
                        })
                );
            }
        }

        [HttpPost]
        public async Task<Guid> CreatePost(CreatePostModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);

            if (userId == default)
            {
                throw new Exception("You are not authorized");
            }
            else
            {
                var metadataModels = model.Attachments;
                List<PostAttachmentModel> postAttachments = new();

                if (metadataModels != null)
                    foreach (var metadata in metadataModels)
                    {
                        postAttachments.Add(new PostAttachmentModel
                        {
                            TempId = metadata.TempId,
                            Name = metadata.Name,
                            MimeType = metadata.MimeType,
                            Size = metadata.Size,
                            FilePath = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", metadata.TempId.ToString()),
                            AuthorId = userId
                        });
                    }

                var postModel = new PostModel
                {
                    AuthorId = userId,
                    CreatedDate = DateTime.UtcNow,
                    Caption = model.Caption,
                    PostAttachments = postAttachments
                };

                postModel.PostAttachments?.ForEach(postAttachmentModel =>
                {
                    var tempFileInfo = new FileInfo(Path.Combine(Path.GetTempPath(), postAttachmentModel.TempId.ToString()));
                    if (!tempFileInfo.Exists)
                    {
                        throw new Exception("File not found");
                    }
                    else
                    {
                        var destFileInfo = new FileInfo(postAttachmentModel.FilePath);

                        if (destFileInfo.Directory != null && !destFileInfo.Directory.Exists)
                        {
                            destFileInfo.Directory.Create();
                        }

                        System.IO.File.Move(tempFileInfo.FullName, postAttachmentModel.FilePath, true);
                    }
                });

                return await _postService.CreatePost(postModel);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<ReturnPostModel>> GetPosts(int skip, int take)
        {
            return await _postService.GetPosts(skip, take);
        }
    }
}