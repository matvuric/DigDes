using Api.Models.Attachment;
using Api.Models.Post;
using Api.Models.User;
using AutoMapper;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class PostService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private Func<UserModel, string?>? _linkAvatarGenerator;
        private Func<AttachmentModel, string?>? _linkAttachmentGenerator;

        public PostService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void SetLinkGenerator(Func<UserModel, string?> linkAvatarGenerator, Func<AttachmentModel, string?> linkAttachmentGenerator)
        {
            _linkAvatarGenerator = linkAvatarGenerator;
            _linkAttachmentGenerator = linkAttachmentGenerator;
        }

        public async Task<Guid> CreatePost(PostModel model)
        {
            var dbPost = _mapper.Map<Post>(model);
            
            var postEntity = await _context.Posts.AddAsync(dbPost);
            await _context.SaveChangesAsync();

            return postEntity.Entity.Id;
        }

        public async Task<IEnumerable<ReturnPostModel>> GetPosts(int skip, int take)
        {
            var posts = await _context.Posts
                .Include(post => post.Author).ThenInclude(user=> user.Avatar)
                .Include(post => post.PostAttachments)
                .AsNoTracking().Skip(skip).Take(take).ToListAsync();
            
            var result = posts.Select(post =>
                new ReturnPostModel
                {
                    Id = post.Id,
                    Caption = post.Caption,
                    Author = new UserAvatarModel(_mapper.Map<UserModel>(post.Author), post.Author.Avatar == null? null: _linkAvatarGenerator),
                    PostAttachments = post.PostAttachments?.Select(postAttachment => 
                        new AttachmentWithLinkModel(_mapper.Map<AttachmentModel>(postAttachment), _linkAttachmentGenerator)).ToList()
                });

            return result;
        }

        public async Task<AttachmentModel> GetPostAttachmentById(Guid postAttachmentId)
        {
            var attachment = await _context.Attachments.FirstOrDefaultAsync(x => x.Id == postAttachmentId);

            return _mapper.Map<AttachmentModel>(attachment);
        }
    }
}
