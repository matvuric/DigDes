using Api.Models;
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
        private readonly AttachService _attachService;
        private readonly PostService _postService;
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public PostController(AttachService attachService, DataContext context, PostService postService, IMapper mapper)
        {
            _attachService = attachService;
            _postService = postService;
            _mapper = mapper;
            _context = context;
        }

        [HttpPost]
        public async Task CreatePost(string text, Guid id, [FromForm] List<IFormFile> files)
        {
            var post = await _context.Posts.AddAsync(new Post()
            {
                Id = Guid.NewGuid(),
                Text = text,
                CreatedDate = DateTime.UtcNow,
                AuthorId = id
            });

            await _context.SaveChangesAsync();

            if (files.Count > 0)
            {
                var attaches = await _attachService.UploadFiles(files);

                await _postService.UploadPostAttaches(attaches, post.Entity.Id, post.Entity.AuthorId);
            }
        }

        [HttpGet]
        public async Task<PostModel> GetPost(Guid id)
        {
            var post = await _postService.GetPostById(id);
            var postAttaches = await _context.PostAttaches.Where(p=>p.Post.Id == id).ToListAsync();
            foreach (var attach in postAttaches)
            {
                post.Attaches.Add(attach);
            }

            return _mapper.Map<PostModel>(post);
        }

        [HttpGet]
        public async Task<FileResult> GetPostAttach(Guid id)
        {
            var attach = await _context.PostAttaches.FirstOrDefaultAsync(x => x.Id == id);

            if (attach == null)
            {
                throw new Exception("Attach is not found");
            }

            return File(await System.IO.File.ReadAllBytesAsync(attach.FilePath), attach.MimeType);
        }
    }
}
