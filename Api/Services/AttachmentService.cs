using Api.Models.Attachment;
using Microsoft.AspNetCore.Mvc;

namespace Api.Services
{
    public class AttachmentService
    {
        public async Task<List<MetadataModel>> UploadFiles([FromForm] List<IFormFile> files)
        {
            var res = new List<MetadataModel>();

            foreach (var file in files)
            {
                res.Add(await UploadFile(file));
            }

            return res;
        }

        public void MoveFile(MetadataLinkModel model)
        {
            var tempFileInfo = new FileInfo(Path.Combine(Path.GetTempPath(), model.TempId.ToString()));

            if (!tempFileInfo.Exists)
            {
                throw new FileNotFoundException();
            }

            var destFileInfo = new FileInfo(model.FilePath);

            if (destFileInfo.Directory is { Exists: false })
            {
                destFileInfo.Directory.Create();
            }

            File.Move(tempFileInfo.FullName, model.FilePath, true);
        }

        private async Task<MetadataModel> UploadFile([FromForm] IFormFile file)
        {
            var tempPath = Path.GetTempPath();
            var meta = new MetadataModel
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

            await using var stream = File.Create(newPath);
            await file.CopyToAsync(stream);

            return meta;
        }
    }
}