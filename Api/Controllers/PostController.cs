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
        public async Task CreatePost(CreatePostModel model)
        {
            if (!model.AuthorId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);

                if (userId == default)
                {
                    throw new UnauthorizedException();
                }

                model.AuthorId = userId;
            }

            await _postService.CreatePost(model);
        }

        [HttpGet]
        public async Task<IEnumerable<ReturnPostModel>> GetPosts(int skip, int take = 10)
        {
            return await _postService.GetPosts(skip, take);
        }

        [HttpGet]
        public async Task<ReturnPostWithCommentsModel> GetPostById(Guid postId)
        {
            var currentUserId = User.GetClaimValue<Guid>(ClaimNames.Id);
            return await _postService.GetPostWithComments(currentUserId, postId);
        }

        [HttpGet]
        public async Task<IEnumerable<ReturnPostModel>> GetCurrentUserPosts(int skip, int take = 10)
        {
            return await _postService.GetUserPosts(User.GetClaimValue<Guid>(ClaimNames.Id), skip, take);
        }

        [HttpGet]
        public async Task<IEnumerable<ReturnPostModel>> GetUserPostsByUserId(Guid userId, int skip, int take = 10)
        {
            var currentUserId = User.GetClaimValue<Guid>(ClaimNames.Id);
            return await _postService.GetUserPostsByUserId(currentUserId, userId, skip, take);
        }

        [HttpGet]
        public async Task<IEnumerable<ReturnPostModel>> GetUserFollowingPosts()
        {
            return await _postService.GetUserFollowingPosts(User.GetClaimValue<Guid>(ClaimNames.Id));
        }

        [HttpPost]
        public async Task CreatePostComment(CreatePostCommentModel model)
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

            await _postCommentService.CreatePostComment(model);
        }

        [HttpGet]
        public async Task<IEnumerable<ReturnPostCommentModel>> GetPostComments(int skip, int take = 10)
        {
            return await _postCommentService.GetPostComments(skip, take);
        }

        [HttpGet]
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
        public async Task<IEnumerable<ReturnPostCommentModel>> GetPostCommentsByPostId(Guid postId, int skip, int take = 10)
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

                model.AuthorId = userId;
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