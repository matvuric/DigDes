using Api.Models;
using Api.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task CreateUser(CreateUserModel model)
        {
            if (await _userService.CheckUserExist(model.Email))
            {
                throw new Exception("User is exist");
            }

            await _userService.CreateUser(model);
        }

        [HttpPost]
        [Authorize]
        public async Task SetAvatar(MetaDataModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;

            if (Guid.TryParse(userIdString, out var userId))
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

                    if (destFileInfo.Directory == null)
                    {
                        throw new Exception("Directory is not defined");
                    }
                    else
                    {
                        if (!destFileInfo.Directory.Exists)
                        {
                            destFileInfo.Directory.Create();
                        }
                    }

                    System.IO.File.Copy(tempFileInfo.FullName, path, true);
                    await _userService.SetAvatar(userId, model, path);
                }
            }
            else
            {
                throw new Exception("You are not authorized");
            }
        }

        [HttpGet]
        public async Task<FileResult> GetUserAvatar(Guid id)
        {
            var avatar = await _userService.GetAvatarById(id);
            return File(await System.IO.File.ReadAllBytesAsync(avatar.FilePath), avatar.MimeType);
        }

        [HttpGet]
        public async Task<FileResult> DownloadUserAvatar(Guid id)
        {
            var avatar = await _userService.GetAvatarById(id);

            HttpContext.Response.ContentType = avatar.MimeType;
            var result = new FileContentResult(await System.IO.File.ReadAllBytesAsync(avatar.FilePath), avatar.MimeType)
            {
                FileDownloadName = avatar.Name
            };

            return result;
        }

        [HttpGet]
        [Authorize]
        public async Task<List<UserModel>> GetUsers() => await _userService.GetUsers();

        [HttpGet]
        [Authorize]
        public async Task<UserModel> GetCurrentUser()
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;

            if (Guid.TryParse(userIdString, out var userId))
            {
                return await _userService.GetUser(userId);
            }
            else
            {
                throw new Exception("You are not authorized");
            }
        }
    }
}
