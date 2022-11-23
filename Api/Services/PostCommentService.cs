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
        private readonly AttachmentService _attachmentService;

        public PostCommentService(DataContext context, IMapper mapper, AttachmentService attachmentService)
        {
            _context = context;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        public async Task CreatePostComment(CreatePostCommentModel createModel)
        {
            var postCommentModel = _mapper.Map<PostCommentModel>(createModel);
            postCommentModel.PostCommentAttachments?.ForEach(model =>
            {
                model.AuthorId = postCommentModel.AuthorId;
                model.FilePath = Path.Combine(Directory.GetCurrentDirectory(),
                    "Attachments", model.TempId.ToString());
                _attachmentService.MoveFile(model);
            });

            var dbPostComment = _mapper.Map<PostComment>(postCommentModel);
            await _context.PostComments.AddAsync(dbPostComment);
            await _context.SaveChangesAsync();
        }

        // TODO : Edit post comment

        public async Task<List<ReturnPostCommentModel>> GetPostComments(int skip, int take)
        {
            var postComments = await _context.PostComments.AsNoTracking()
                .Include(comm => comm.Author).ThenInclude(user => user.Avatar)
                .Include(comm => comm.Author).ThenInclude(user => user.Followers)
                .Include(comm => comm.Author).ThenInclude(user => user.Following)
                .Include(comm => comm.PostCommentAttachments)
                .Include(comm => comm.Likes)
                .OrderByDescending(comm => comm.CreatedDate).Skip(skip).Take(take)
                .Select(comm => _mapper.Map<ReturnPostCommentModel>(comm)).ToListAsync();

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
                .Include(comm => comm.Author).ThenInclude(user => user.Avatar)
                .Include(comm => comm.Author).ThenInclude(user => user.Followers)
                .Include(comm => comm.Author).ThenInclude(user => user.Following)
                .Include(comm => comm.PostCommentAttachments)
                .Include(comm => comm.Likes)
                .FirstOrDefaultAsync(comm => comm.Id == id);

            if (postComment == null)
            {
                throw new PostCommentNotFoundException();
            }

            return postComment;
        }

        public async Task<List<ReturnPostCommentModel>> GetCurrentUserPostComments(Guid userId, int skip, int take)
        {
            var postComments = await _context.PostComments.AsNoTracking()
                .Where(comm => comm.AuthorId == userId)
                .Include(comm => comm.Author).ThenInclude(user => user.Avatar)
                .Include(comm => comm.Author).ThenInclude(user => user.Followers)
                .Include(comm => comm.Author).ThenInclude(user => user.Following)
                .Include(comm => comm.PostCommentAttachments)
                .Include(comm => comm.Likes)
                .OrderByDescending(comm => comm.CreatedDate).Skip(skip).Take(take)
                .Select(comm => _mapper.Map<ReturnPostCommentModel>(comm)).ToListAsync();

            return postComments;
        }

        public async Task<List<ReturnPostCommentModel>> GetPostCommentsByPostId(Guid postId, int skip, int take)
        {
            var postComments = await _context.PostComments.AsNoTracking()
                .Where(comm => comm.PostId == postId)
                .Include(comm => comm.Author).ThenInclude(user => user.Avatar)
                .Include(comm => comm.Author).ThenInclude(user => user.Followers)
                .Include(comm => comm.Author).ThenInclude(user => user.Following)
                .Include(comm => comm.PostCommentAttachments)
                .Include(comm => comm.Likes)
                .OrderByDescending(comm => comm.CreatedDate).Skip(skip).Take(take)
                .Select(comm => _mapper.Map<ReturnPostCommentModel>(comm)).ToListAsync();

            return postComments;
        }

        public async Task DeletePostComment(Guid id)
        {
            var dbPostComment = await GetPostCommentById(id);
            _context.PostComments.Remove(dbPostComment);
            await _context.SaveChangesAsync();
        }

        public async Task<AttachmentModel> GetPostCommentAttachmentById(Guid postCommentAttachmentId)
        {
            var attachment = await _context.Attachments
                .FirstOrDefaultAsync(attachment => attachment.Id == postCommentAttachmentId);

            return _mapper.Map<AttachmentModel>(attachment);
        }
    }
}
