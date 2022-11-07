using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly AttachService _attachService;

        public PostController(AttachService attachService)
        {
            _attachService = attachService;
        }

        [HttpPost]
        public async Task<CreatePostModel> CreatePost(string text, Guid userId, [FromForm] List<IFormFile> files)
        {
            var res = new CreatePostModel
            {
                Text = text,
                UserId = userId,
                Attaches = await _attachService.UploadFiles(files)
            };

            return res;
        }
    }
}
