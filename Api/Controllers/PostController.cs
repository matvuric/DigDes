using Api.Models.Post;
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
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;

        public PostController(PostService postService)
        {
            _postService = postService;

            _postService.SetLinkGenerator(userId => 
                    Url.ControllerAction<AttachmentController>(nameof(AttachmentController.GetUserAvatar), new { userId }), 
                postAttachmentId => 
                    Url.ControllerAction<AttachmentController>(nameof(AttachmentController.GetPostAttachment), new { postAttachmentId })
            );
        }

        [HttpPost]
        public async Task<Guid> CreatePost(CreatePostModel model)
        {
            if (!model.AuthorId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);

                if (userId == default)
                {
                    throw new Exception("You are not authorized");
                }
                else
                {
                    model.AuthorId = userId;
                }
            }

            return await _postService.CreatePost(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<ReturnPostModel>> GetPosts(int skip, int take = 10)
        {
            return await _postService.GetPosts(skip, take);
        }
    }
}