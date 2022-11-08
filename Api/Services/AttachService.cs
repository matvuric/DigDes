using Api.Models.Attach;
using Microsoft.AspNetCore.Mvc;

namespace Api.Services
{
    public class AttachService
    {
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

                await using (var stream = File.Create(newPath))
                {
                    await file.CopyToAsync(stream);
                }

                return meta;
            }
        }
    }
}