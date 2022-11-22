using Api.Models.Attachment;
using Api.Models.Post;
using AutoMapper;
using Common.Exceptions;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using FileNotFoundException = Common.Exceptions.FileNotFoundException;

namespace Api.Services
{
    public class PostService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly AttachmentService _attachmentService;

        public PostService(DataContext context, IMapper mapper, AttachmentService attachmentService)
        {
            _context = context;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        public async Task CreatePost(CreatePostModel createModel)
        {
            var postModel = _mapper.Map<PostModel>(createModel);
            postModel.PostAttachments?.ForEach(model =>
            {
                model.AuthorId = postModel.AuthorId;
                model.FilePath = Path.Combine(Directory.GetCurrentDirectory(),
                    "Attachments", model.TempId.ToString());
                _attachmentService.MoveFile(model);
            });

            var dbPost = _mapper.Map<Post>(postModel);
            await _context.Posts.AddAsync(dbPost);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ReturnPostModel>> GetPosts(int skip, int take)
        {
            var posts = await _context.Posts.AsNoTracking()
                .Include(post => post.Author).ThenInclude(user => user.Avatar)
                .Include(post => post.Author).ThenInclude(user => user.Followers)
                .Include(post => post.Author).ThenInclude(user => user.Following)
                .Include(post => post.PostAttachments)
                .Include(post => post.Comments)
                .Include(post => post.Likes)
                .OrderByDescending(post => post.CreatedDate).Skip(skip).Take(take)
                .Select(post => _mapper.Map<ReturnPostModel>(post)).ToListAsync();

            return posts;
        }

        public async Task<ReturnPostWithCommentsModel> GetPostWithComments(Guid currentUserId, Guid postId)
        {
            var post = await GetPostById(currentUserId, postId);

            return _mapper.Map<ReturnPostWithCommentsModel>(post);
        }

        public async Task<Post> GetPostById(Guid currentUserId, Guid postId)
        {
            var post = await _context.Posts
                .Include(post => post.Author).ThenInclude(user => user.Avatar)
                .Include(post => post.Author).ThenInclude(user => user.Followers)
                .Include(post => post.Author).ThenInclude(user => user.Following)
                .Include(post => post.PostAttachments)
                .Include(post => post.Comments)!.ThenInclude(comm => comm.PostCommentAttachments)
                .Include(post => post.Comments)!.ThenInclude(comm => comm.Author).ThenInclude(user => user.Avatar)
                .Include(post => post.Comments)!.ThenInclude(comm => comm.Author).ThenInclude(user => user.Followers)
                .Include(post => post.Comments)!.ThenInclude(comm => comm.Author).ThenInclude(user => user.Following)
                .Include(post => post.Comments)!.ThenInclude(comm => comm.Likes)
                .Include(post => post.Likes)
                .FirstOrDefaultAsync(post => post.Id == postId);

            if (post == null)
            {
                throw new PostNotFoundException();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(user => user.Id == post.AuthorId);

            var relation = await _context.Relations
                .Include(rel => rel.FollowingUser)
                .FirstOrDefaultAsync(rel => rel.FollowerId == currentUserId && rel.FollowingId == user!.Id);

            if (relation != null)
            {
                if (relation.FollowingUser.IsPrivate)
                {
                    if (!relation.IsConfirmed)
                    {
                        throw new Exception("This acc is private");
                    }
                }
            }
            else
            {
                if (user!.IsPrivate)
                {
                    throw new Exception("This acc is private");
                }
            }

            if (post == null)
            {
                throw new PostNotFoundException();
            }

            return post;
        }

        public async Task<List<ReturnPostModel>> GetUserPostsByUserId(Guid currentUserId, Guid userId, int skip, int take)
        {
            var relation = await _context.Relations
                .Include(rel => rel.FollowingUser)
                .FirstOrDefaultAsync(rel =>
                    rel.FollowerId == currentUserId && rel.FollowingId == userId);

            if (relation != null)
            {
                if (relation.FollowingUser.IsPrivate)
                {
                    if (!relation.IsConfirmed)
                    {
                        throw new Exception("This acc is private");
                    }
                }
            }
            else
            {
                var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);

                if (user!.IsPrivate)
                {
                    throw new Exception("This acc is private");
                }
            }

            var posts = await GetUserPosts(userId, skip, take);

            return posts;
        }

        public async Task<List<ReturnPostModel>> GetUserPosts(Guid userId, int skip, int take)
        {
            var posts = await _context.Posts.AsNoTracking()
                .Where(post => post.AuthorId == userId)
                .Include(post => post.Author).ThenInclude(user => user.Avatar)
                .Include(post => post.Author).ThenInclude(user => user.Followers)
                .Include(post => post.Author).ThenInclude(user => user.Following)
                .Include(post => post.PostAttachments)
                .Include(post => post.Comments)
                .Include(post => post.Likes)
                .OrderByDescending(post => post.CreatedDate).Skip(skip).Take(take)
                .Select(post => _mapper.Map<ReturnPostModel>(post)).ToListAsync();

            return posts;
        }

        public async Task<List<ReturnPostModel>> GetUserFollowingPosts(Guid userId)
        {
            var user = await _context.Users
                .Include(user => user.Following)!.ThenInclude(rel => rel.FollowingUser).ThenInclude(user => user.Avatar)
                .Include(user => user.Following)!.ThenInclude(rel => rel.FollowingUser).ThenInclude(user => user.Posts)!.ThenInclude(post => post.PostAttachments)
                .Include(user => user.Following)!.ThenInclude(rel => rel.FollowingUser).ThenInclude(user => user.Posts)!.ThenInclude(post => post.Comments)
                .Include(user => user.Following)!.ThenInclude(rel => rel.FollowingUser).ThenInclude(user => user.Posts)!.ThenInclude(post => post.Likes)
                .FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            var followings = user.Following;

            if (followings == null)
            {
                throw new Exception("You do not have followings");
            }

            var posts = new List<ReturnPostModel>();

            foreach (var following in followings)
            {
                var followingPosts = following.FollowingUser.Posts;

                if (followingPosts == null)
                {
                    throw new Exception("There is no posts");
                }

                foreach (var post in followingPosts)
                {
                    posts.Add(_mapper.Map<ReturnPostModel>(post));
                }
            }

            return posts;
        }

        public async Task<AttachmentModel> GetPostAttachmentById(Guid postAttachmentId)
        {
            var attachment = await _context.Attachments
                .FirstOrDefaultAsync(attachment => attachment.Id == postAttachmentId);

            return _mapper.Map<AttachmentModel>(attachment);
        }
    }
}
