using System.Net.Mime;
using Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttachController : ControllerBase
    {
        [HttpPost]
        public async Task<List<MetaDataModel>> UploadFiles([FromForm] List<IFormFile> files)
        {
            var res = new List<MetaDataModel>();

            foreach (var file in files)
            {
                res.Add(await UploadFile(file));
            }

            return res;
        }

        private async Task<MetaDataModel> UploadFile([FromForm] IFormFile file)
        {
            var tempPath = Path.GetTempPath();
            var meta = new MetaDataModel
            {
                TempId = Guid.NewGuid(),
                Name = file.FileName,
                MimeType = file.ContentType,
                Size = file.Length
            };

            var newPath = Path.Combine(tempPath, meta.TempId.ToString());

            var fileInfo = new FileInfo(newPath);
            if (fileInfo.Exists)
            {
                throw new Exception("File already exists");
            }
            else
            {
                if (fileInfo.Directory == null)
                {
                    throw new Exception("Temp is not defined");
                }
                else
                {
                    if (!fileInfo.Directory.Exists)
                    {
                        fileInfo.Directory.Create();
                    }
                }

                await using (var stream = System.IO.File.Create(newPath))
                {
                    await file.CopyToAsync(stream);
                }

                return meta;
            }
        }
    }
}
