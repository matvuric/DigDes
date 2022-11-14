﻿using Api.Services;
using Common.Consts;
using Common.Extentions;

namespace Api.Middlewares.TokenValidator
{
    public class TokenValidatorMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidatorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AuthService authService)
        {
            var isOk = true;
            var sessionId = context.User.GetClaimValue<Guid>(ClaimNames.SessionId);
            if (sessionId != default)
            {
                var session = await authService.GetSessionById(sessionId);
                if (!session.IsActive)
                {
                    isOk = false;
                    context.Response.Clear();
                    context.Response.StatusCode = 401;
                }
            }

            if (isOk)
            {
                await _next(context);
            }
        }
    }
}
