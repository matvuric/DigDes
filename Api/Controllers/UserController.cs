using Api.Models.Attachment;
using Api.Models.User;
using Api.Services;
using Common.Consts;
using Common.Exceptions;
using Common.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FileNotFoundException = Common.Exceptions.FileNotFoundException;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Api")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService, LinkGeneratorService linkGeneratorService)
        {
            _userService = userService;

            linkGeneratorService.LinkAvatarGenerator = userId => 
                Url.ControllerAction<AttachmentController>(nameof(AttachmentController.GetUserAvatar), new
                    {
                        userId
                    });
        }

        /*[HttpPost]
        public async Task EditProfile(EditUserProfileModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);

            if (userId == default)
            {
                throw new UnauthorizedException();
            }

            await _userService.EditProfile(model, userId);
        }*/

        [HttpPost]
        public async Task SetAvatar(MetadataModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);

            if (userId == default)
            {
                throw new UnauthorizedException();
            }
            else
            {
                var tempFileInfo = new FileInfo(Path.Combine(Path.GetTempPath(), model.TempId.ToString()));
                if (!tempFileInfo.Exists)
                {
                    throw new FileNotFoundException();
                }
                else
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", model.TempId.ToString());
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
        public async Task<IEnumerable<UserAvatarModel>> GetUsers()
        {
            return await _userService.GetUsers();   
        }

        [HttpGet]
        public async Task<UserModel> GetCurrentUser()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);

            if (userId == default)
            {
                throw new UnauthorizedException();
            }
            else
            {
                return await _userService.GetUser(userId);
            }
        }

        [HttpGet]
        public async Task<UserAvatarModel> GetUserById(Guid userId)
        {
            return await _userService.GetUser(userId);
        }
    }
}
