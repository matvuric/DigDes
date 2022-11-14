﻿using Api.Models;
using Api.Models.User;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
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
            => await _authService.GetToken(model.Login, model.Password);

        [HttpPost]
        public async Task<TokenModel> RefreshToken(RefreshTokenRequestModel model)
            => await _authService.GetTokenByRefreshToken(model.RefreshToken);

        [HttpPost]
        public async Task<Guid> RegisterUser(CreateUserModel model)
        {
            if (await _userService.CheckUserExist(model.Email))
            {
                throw new Exception("User exists");
            }

            return await _userService.CreateUser(model);
        }


    }
}
