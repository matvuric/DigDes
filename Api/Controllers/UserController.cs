using Api.Models.Attach;
using Api.Models.User;
using Api.Services;
using Common.Consts;
using Common.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;

            if (userService != null)
            {
                _userService.SetLinkGenerator(user =>
                    Url.Action(nameof(GetUserAvatar), new { userId = user.Id, download = false }));
            }
        }

        [HttpPost]
        public async Task SetAvatar(MetaDataModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);

            if (userId == default)
            {
                throw new Exception("You are not authorized");
            }
            else
            {
                var tempFileInfo = new FileInfo(Path.Combine(Path.GetTempPath(), model.TempId.ToString()));
                if (!tempFileInfo.Exists)
                {
                    throw new Exception("File not found");
                }
                else
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Attaches", model.TempId.ToString());
                    var destFileInfo = new FileInfo(path);

                    if (destFileInfo.Directory != null && !destFileInfo.Directory.Exists)
                    {
                        destFileInfo.Directory.Create();
                    }

                    System.IO.File.Copy(tempFileInfo.FullName, path, true);
                    await _userService.SetAvatar(userId, model, path);
                }
            }
        }

        [HttpGet]
        public async Task<FileStreamResult> GetUserAvatar(Guid userId, bool download = false)
        {
            var attach = await _userService.GetAvatarById(userId);
            var fileStream = new FileStream(attach.FilePath, FileMode.Open);

            if (download)
            {
                return File(fileStream, attach.MimeType, attach.Name);
            }
            else
            {
                return File(fileStream, attach.MimeType);
            }
        }

        [HttpGet]
        public async Task<FileStreamResult> GetCurrentUserAvatar(bool download = false)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);

            if (userId == default)
            {
                throw new Exception("You are not authorized");
            }
            else
            {
                return await GetUserAvatar(userId, download);
            }
        }

        [HttpGet]
        public async Task<IEnumerable<UserAvatarModel>> GetUsers() => await _userService.GetUsers();

        [HttpGet]
        public async Task<UserModel> GetCurrentUser()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);

            if (userId == default)
            {
                throw new Exception("You are not authorized");
            }
            else
            {
                return await _userService.GetUser(userId);
            }
        }
    }
}
