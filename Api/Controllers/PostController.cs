using Api.Models.Post;
using Api.Services;
using AutoMapper;
using DAL;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly AttachmentService _attachmentService;
        private readonly PostService _postService;
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public PostController(AttachmentService attachmentService, DataContext context, PostService postService, IMapper mapper)
        {
            _attachmentService = attachmentService;
            _postService = postService;
            _mapper = mapper;
            _context = context;
        }

        [HttpPost]
        public async Task CreatePost(string caption, Guid id, [FromForm] List<IFormFile> files)
        {
            var post = await _context.Posts.AddAsync(new Post()
            {
                Id = Guid.NewGuid(),
                Caption = caption,
                CreatedDate = DateTime.UtcNow,
                AuthorId = id
            });

            await _context.SaveChangesAsync();

            if (files.Count > 0)
            {
                var attachments = await _attachmentService.UploadFiles(files);

                await _postService.UploadPostAttachments(attachments, post.Entity.Id, post.Entity.AuthorId);
            }
        }

        [HttpGet]
        public async Task<PostModel> GetPost(Guid id)
        {
            var post = await _postService.GetPostById(id);
            var postAttachments = await _context.PostAttachments.Where(p=>p.Post.Id == id).ToListAsync();
            foreach (var attachment in postAttachments)
            {
                post.PostAttachments.Add(attachment);
            }

            return _mapper.Map<PostModel>(post);
        }

        [HttpGet]
        public async Task<FileResult> GetPostAttachment(Guid id)
        {
            var attachment = await _context.PostAttachments.FirstOrDefaultAsync(x => x.Id == id);

            if (attachment == null)
            {
                throw new Exception("Attachment is not found");
            }

            return File(await System.IO.File.ReadAllBytesAsync(attachment.FilePath), attachment.MimeType);
        }
    }
}
