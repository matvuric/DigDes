using Api.Models.Token;
using Api.Models.User;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;

        public AuthController(AuthService authService, UserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<TokenModel> Token(TokenRequestModel model)
        {
            return await _authService.GetToken(model.Login, model.Password);
        }

        [HttpPost]
        public async Task<TokenModel> RefreshToken(RefreshTokenRequestModel model)
        {
            return await _authService.GetTokenByRefreshToken(model.RefreshToken);
        }

        [HttpPost]
        public async Task RegisterUser(CreateUserModel model)
        {
            if (await _userService.CheckUserExist(model.Email))
            {
                throw new Exception("User exists");
            }

            await _userService.CreateUser(model);
        }
    }
}
