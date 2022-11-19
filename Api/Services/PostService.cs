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

        public PostService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
            var posts = await _context.Posts
                .Include(post => post.Author).ThenInclude(user => user.Avatar)
                .Include(post => post.PostAttachments).AsNoTracking()
                .OrderByDescending(post => post.CreatedDate).Skip(skip).Take(take)
                .Select(post => _mapper.Map<ReturnPostModel>(post)).ToListAsync();

            return posts;
        }

        public async Task<ReturnPostModel> GetPost(Guid postId)
        {
            var post = await GetPostById(postId);

            return _mapper.Map<ReturnPostModel>(post);
        }

        public async Task<Post> GetPostById(Guid id)
        {
            var post = await _context.Posts.Include(post => post.Author).ThenInclude(user => user.Avatar)
                .Include(post => post.PostAttachments).FirstOrDefaultAsync(post => post.Id == id);

            if (post == null || post == default)
            {
                throw new PostNotFoundException();
            }

            return post;
        }

        public async Task<List<ReturnPostModel>> GetCurrentUserPosts(Guid userId, int skip, int take)
        {
            var posts = await _context.Posts.Where(post => post.AuthorId == userId)
                .Include(post => post.Author).ThenInclude(user => user.Avatar)
                .Include(post => post.PostAttachments).AsNoTracking()
                .OrderByDescending(post => post.CreatedDate).Skip(skip).Take(take)
                .Select(post => _mapper.Map<ReturnPostModel>(post)).ToListAsync();

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
