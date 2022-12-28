using Api.Models.Push;
using Api.Services;
using Common.Consts;
using Common.Exceptions;
using Common.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PushController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly GooglePushService _googlePushService;

        public PushController (UserService userService, GooglePushService googlePushService)
        {
            _userService = userService;
            _googlePushService = googlePushService;
        }

        [HttpPost]
        public async Task Subscribe(PushTokenModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);

            if (userId == default)
            {
                throw new UnauthorizedException();
            }

            await _userService.SetPushToken(userId, model.Token);
        }

        [HttpDelete]
        public async Task Unsubscribe()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);

            if (userId == default)
            {
                throw new UnauthorizedException();
            }

            await _userService.SetPushToken(userId);
        }

        [HttpPost]
        public async Task<List<string>> SendPush(SendPushModel model)
        {
            var userId = model.UserId ?? User.GetClaimValue<Guid>(ClaimNames.Id);

            if (userId == default)
            {
                throw new UnauthorizedException();
            }

            var token = await _userService.GetPushToken(userId);

            if (token == default)
            {
                throw new Exception("Token not found");
            }

            return _googlePushService.SendNotification(token, model.Push);
        }
    }
}
