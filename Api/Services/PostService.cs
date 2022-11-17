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
        private Func<Guid, string?>? _linkAvatarGenerator;
        private Func<Guid, string?>? _linkAttachmentGenerator;

        public PostService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void SetLinkGenerator(Func<Guid, string?> linkAvatarGenerator, Func<Guid, string?> linkAttachmentGenerator)
        {
            _linkAvatarGenerator = linkAvatarGenerator;
            _linkAttachmentGenerator = linkAttachmentGenerator;
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
                    throw new Exception("File not found");
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
                    Author = _mapper.Map<User, UserAvatarModel>(post.Author, opts => opts.AfterMap(FixAvatar)),
                    PostAttachments = post.PostAttachments?.Select(postAttachment => 
                        _mapper.Map<PostAttachment, AttachmentExternalModel>(postAttachment, opts=> opts.AfterMap(FixAttachment))).ToList()
                });

            return result;
        }

        public async Task<AttachmentModel> GetPostAttachmentById(Guid postAttachmentId)
        {
            var attachment = await _context.Attachments.FirstOrDefaultAsync(x => x.Id == postAttachmentId);

            return _mapper.Map<AttachmentModel>(attachment);
        }

        private void FixAvatar(User src, UserAvatarModel dest)
        {
            dest.AvatarLink = src.Avatar == null ? null : _linkAvatarGenerator?.Invoke(src.Id);
        }

        private void FixAttachment(PostAttachment src, AttachmentExternalModel dest)
        {
            dest.AttachmentLink = _linkAttachmentGenerator?.Invoke(src.Id);
        }
    }
}
