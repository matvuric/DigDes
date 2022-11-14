using Api.Models.Attach;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AttachController : ControllerBase
    {
        private readonly AttachService _attachService;

        public AttachController(AttachService attachService)
        {
            _attachService = attachService;
        }

        [HttpPost]
        public async Task<List<MetaDataModel>> UploadFiles([FromForm] List<IFormFile> files)
        {
            return await _attachService.UploadFiles(files);
        }
    }
}