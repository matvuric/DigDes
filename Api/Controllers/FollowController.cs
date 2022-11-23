using Api.Models.Follow;
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
    [ApiExplorerSettings(GroupName = "Api")]
    public class FollowController : ControllerBase
    {
        private readonly FollowService _followService;

        public FollowController(FollowService followService)
        {
            _followService = followService;
        }

        [HttpPost]
        public async Task FollowUser(FollowModel model)
        {
            if (!model.FollowerId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);

                if (userId == default)
                {
                    throw new UnauthorizedException();
                }
                
                model.FollowerId = userId;
            }
            await _followService.FollowUser(model);
        }

        [HttpPost]
        public async Task UnfollowUser(FollowModel model)
        {
            if (!model.FollowerId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);

                if (userId == default)
                {
                    throw new UnauthorizedException();
                }

                model.FollowerId = userId;
            }
            await _followService.UnfollowUser(model);
        }

        [HttpPost]
        public async Task ConfirmFollow(FollowModel model)
        {
            await _followService.ConfirmFollow(model);
        }
    }
}
