using Api.Models.Attachment;
using Api.Models.PostComment;
using AutoMapper;
using Common.Exceptions;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class PostCommentService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public PostCommentService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Guid> CreatePostComment(CreatePostCommentModel createModel)
        {
            var postCommentModel = _mapper.Map<PostCommentModel>(createModel);
            postCommentModel.PostCommentAttachments?.ForEach(attachmentModel =>
            {
                attachmentModel.AuthorId = postCommentModel.AuthorId;
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

            var dbPostComment = _mapper.Map<PostComment>(postCommentModel);

            var postCommentEntity = await _context.PostComments.AddAsync(dbPostComment);
            await _context.SaveChangesAsync();

            return postCommentEntity.Entity.Id;
        }

        public async Task<List<ReturnPostCommentModel>> GetPostComments(int skip, int take)
        {
            var postComments = await _context.PostComments.AsNoTracking()
                .Include(postComment => postComment.Author).ThenInclude(user => user.Avatar)
                .Include(postComment => postComment.Author).ThenInclude(user => user.Posts)
                .Include(postComment => postComment.Author).ThenInclude(user => user.Likes)
                .Include(postComment => postComment.PostCommentAttachments)
                .Include(postComment => postComment.Likes)
                .OrderByDescending(postComment => postComment.CreatedDate).Skip(skip).Take(take)
                .Select(postComment => _mapper.Map<ReturnPostCommentModel>(postComment)).ToListAsync();

            return postComments;
        }

        public async Task<ReturnPostCommentModel> GetPostComment(Guid postCommentId)
        {
            var postComment = await GetPostCommentById(postCommentId);

            return _mapper.Map<ReturnPostCommentModel>(postComment);
        }

        public async Task<PostComment> GetPostCommentById(Guid id)
        {
            var postComment = await _context.PostComments
                .Include(postComment => postComment.Author).ThenInclude(user => user.Avatar)
                .Include(postComment => postComment.Author).ThenInclude(user => user.Posts)
                .Include(postComment => postComment.Author).ThenInclude(user => user.Likes)
                .Include(postComment => postComment.PostCommentAttachments)
                .Include(postComment => postComment.Likes)
                .FirstOrDefaultAsync(postComment => postComment.Id == id);

            if (postComment == null || postComment == default)
            {
                throw new PostCommentNotFoundException();
            }

            return postComment;
        }

        public async Task<List<ReturnPostCommentModel>> GetCurrentUserPostComments(Guid userId, int skip, int take)
        {
            var postComments = await _context.PostComments.AsNoTracking()
                .Where(postComment => postComment.AuthorId == userId)
                .Include(postComment => postComment.Author).ThenInclude(user => user.Avatar)
                .Include(postComment => postComment.Author).ThenInclude(user => user.Posts)
                .Include(postComment => postComment.Author).ThenInclude(user => user.Likes)
                .Include(postComment => postComment.PostCommentAttachments)
                .Include(postComment => postComment.Likes)
                .OrderByDescending(postComment => postComment.CreatedDate).Skip(skip).Take(take)
                .Select(postComment => _mapper.Map<ReturnPostCommentModel>(postComment)).ToListAsync();

            return postComments;
        }

        public async Task<List<ReturnPostCommentModel>> GetPostCommentsByPostId(Guid postId, int skip, int take)
        {
            var postComments = await _context.PostComments.AsNoTracking()
                .Where(postComment => postComment.PostId == postId)
                .Include(postComment => postComment.Author).ThenInclude(user => user.Avatar)
                .Include(postComment => postComment.Author).ThenInclude(user => user.Posts)
                .Include(postComment => postComment.Author).ThenInclude(user => user.Likes)
                .Include(postComment => postComment.PostCommentAttachments)
                .Include(postComment => postComment.Likes)
                .OrderByDescending(postComment => postComment.CreatedDate).Skip(skip).Take(take)
                .Select(postComment => _mapper.Map<ReturnPostCommentModel>(postComment)).ToListAsync();

            return postComments;
        }

        public async Task<AttachmentModel> GetPostCommentAttachmentById(Guid postCommentAttachmentId)
        {
            var attachment = await _context.Attachments
                .FirstOrDefaultAsync(attachment => attachment.Id == postCommentAttachmentId);

            return _mapper.Map<AttachmentModel>(attachment);
        }
    }
}
