using Api.Models.Like;
using Api.Models.Post;
using Api.Models.PostComment;
using Api.Services;
using Common.Consts;
using Common.Exceptions;
using Common.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Api")]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly PostCommentService _postCommentService;
        private readonly LikeService _likeService;

        public PostController(PostService postService, PostCommentService postCommentService, LikeService likeService, LinkGeneratorService linkGeneratorService)
        {
            _postService = postService;
            _postCommentService = postCommentService;
            _likeService = likeService;

            linkGeneratorService.LinkAvatarGenerator = userId =>  
                Url.ControllerAction<AttachmentController>(nameof(AttachmentController.GetUserAvatar), new
                    {
                        userId
                    });
            linkGeneratorService.LinkAttachmentGenerator = postAttachmentId => 
                Url.ControllerAction<AttachmentController>(nameof(AttachmentController.GetPostAttachment), new
                    {
                        postAttachmentId
                    });
        }

        [HttpPost]
        public async Task<Guid> CreatePost(CreatePostModel model)
        {
            if (!model.AuthorId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);

                if (userId == default)
                {
                    throw new UnauthorizedException();
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<ReturnPostWithCommentsModel> GetPostById(Guid postId)
        {
            return await _postService.GetPostWithComments(postId);
        }

        [HttpGet]
        public async Task<IEnumerable<ReturnPostModel>> GetCurrentUserPosts(int skip, int take = 10)
        {
            return await _postService.GetCurrentUserPosts(User.GetClaimValue<Guid>(ClaimNames.Id), skip, take);
        }

        [HttpGet]
        public async Task<IEnumerable<ReturnPostModel>> GetUserFollowingPosts(int skip, int take = 10)
        {
            return await _postService.GetUserFollowingPosts(User.GetClaimValue<Guid>(ClaimNames.Id), skip, take);
        }

        [HttpPost]
        public async Task<Guid> CreatePostComment(CreatePostCommentModel model)
        {
            if (!model.AuthorId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);

                if (userId == default)
                {
                    throw new UnauthorizedException();
                }
                else
                {
                    model.AuthorId = userId;
                }
            }

            return await _postCommentService.CreatePostComment(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<ReturnPostCommentModel>> GetPostComments(int skip, int take = 10)
        {
            return await _postCommentService.GetPostComments(skip, take);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ReturnPostCommentModel> GetPostCommentById(Guid postCommentId)
        {
            return await _postCommentService.GetPostComment(postCommentId);
        }

        [HttpGet]
        public async Task<IEnumerable<ReturnPostCommentModel>> GetCurrentUserPostComments(int skip, int take = 10)
        {
            return await _postCommentService.GetCurrentUserPostComments(User.GetClaimValue<Guid>(ClaimNames.Id), skip, take);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<ReturnPostCommentModel>> GetPostCommentsById(Guid postId, int skip, int take = 10)
        {
            return await _postCommentService.GetPostCommentsByPostId(postId, skip, take);
        }

        [HttpPost]
        public async Task LikePost(PostLikeModel model)
        {
            if (!model.AuthorId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);

                if (userId == default)
                {
                    throw new UnauthorizedException();
                }
                else
                {
                    model.AuthorId = userId;
                }
            }

            await _likeService.LikePost(model);
        }

        [HttpPost]
        public async Task LikePostComment(PostCommentLikeModel model)
        {
            if (!model.AuthorId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);

                if (userId == default)
                {
                    throw new UnauthorizedException();
                }
                else
                {
                    model.AuthorId = userId;
                }
            }

            await _likeService.LikePostComment(model);
        }
    }
}