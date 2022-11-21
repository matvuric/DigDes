using Api.Models.Attachment;
using Api.Models.Post;
using AutoMapper;
using Common.Exceptions;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class PostService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly UserService _userService;

        public PostService(DataContext context, IMapper mapper, UserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<Guid> CreatePost(CreatePostModel createModel)
        {
            var postModel = _mapper.Map<PostModel>(createModel);
            postModel.PostAttachments?.ForEach(attachmentModel =>
            {
                attachmentModel.AuthorId = postModel.AuthorId;
                attachmentModel.FilePath = Path.Combine(Directory.GetCurrentDirectory(),
                    "Attachments", attachmentModel.TempId.ToString());

                var tempFileInfo = new FileInfo(Path.Combine(Path.GetTempPath(),
                    attachmentModel.TempId.ToString()));

                if (!tempFileInfo.Exists)
                {
                    throw new Common.Exceptions.FileNotFoundException();
                }
                else
                {
                    var destFileInfo = new FileInfo(attachmentModel.FilePath);

                    if (destFileInfo.Directory != null && !destFileInfo.Directory.Exists)
                    {
                        destFileInfo.Directory.Create();
                    }

                    File.Move(tempFileInfo.FullName, attachmentModel.FilePath, true);
                }
            });

            var dbPost = _mapper.Map<Post>(postModel);

            var postEntity = await _context.Posts.AddAsync(dbPost);
            await _context.SaveChangesAsync();

            return postEntity.Entity.Id;
        }

        public async Task<List<ReturnPostModel>> GetPosts(int skip, int take)
        {
            var posts = await _context.Posts.AsNoTracking()
                .Include(post => post.Author).ThenInclude(user => user.Avatar)
                .Include(post => post.PostAttachments)
                .Include(post => post.Comments)
                .Include(post => post.Likes)
                .OrderByDescending(post => post.CreatedDate).Skip(skip).Take(take)
                .Select(post => _mapper.Map<ReturnPostModel>(post)).ToListAsync();

            return posts;
        }

        public async Task<ReturnPostModel> GetPost(Guid postId)
        {
            var post = await GetPostById(postId);

            return _mapper.Map<ReturnPostModel>(post);
        }

        public async Task<ReturnPostWithCommentsModel> GetPostWithComments(Guid postId)
        {
            var post = await GetPostById(postId);

            return _mapper.Map<ReturnPostWithCommentsModel>(post);
        }

        public async Task<Post> GetPostById(Guid id)
        {
            var post = await _context.Posts
                .Include(post => post.Author).ThenInclude(user => user.Avatar)
                .Include(post => post.PostAttachments)
                .Include(post => post.Comments)!.ThenInclude(postComment => postComment.PostCommentAttachments)
                .Include(post => post.Comments)!.ThenInclude(postComment => postComment.Author).ThenInclude(user => user.Avatar)
                .Include(post => post.Comments)!.ThenInclude(postComment => postComment.Likes)
                .Include(post => post.Likes)
                .FirstOrDefaultAsync(post => post.Id == id);

            if (post == null || post == default)
            {
                throw new PostNotFoundException();
            }

            return post;
        }

        public async Task<List<ReturnPostModel>> GetCurrentUserPosts(Guid userId, int skip, int take)
        {
            var posts = await _context.Posts.AsNoTracking()
                .Where(post => post.AuthorId == userId)
                .Include(post => post.Author).ThenInclude(user => user.Avatar)
                .Include(post => post.PostAttachments)
                .Include(post => post.Comments)
                .Include(post => post.Likes)
                .OrderByDescending(post => post.CreatedDate).Skip(skip).Take(take)
                .Select(post => _mapper.Map<ReturnPostModel>(post)).ToListAsync();

            return posts;
        }

        public async Task<List<ReturnPostModel>> GetUserFollowingPosts(Guid userId, int skip, int take)
        {
            var user = await _context.Users
                .Include(user => user.Following)!.ThenInclude(rel => rel.FollowingUser).ThenInclude(user => user!.Posts)!.ThenInclude(post => post.PostAttachments)
                .Include(user => user.Following)!.ThenInclude(rel => rel.FollowingUser).ThenInclude(user => user!.Posts)!.ThenInclude(post => post.Comments)
                .Include(user => user.Following)!.ThenInclude(rel => rel.FollowingUser).ThenInclude(user => user!.Posts)!.ThenInclude(post => post.Likes)
                .FirstOrDefaultAsync(user => user.Id == userId);

            var a = user!.Following;
            var posts = new List<ReturnPostModel>();
            foreach (var u in a)
            {
                var followingPosts = u.FollowingUser!.Posts;
                foreach (var post in followingPosts!)
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
