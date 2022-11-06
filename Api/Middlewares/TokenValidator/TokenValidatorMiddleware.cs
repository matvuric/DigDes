using Api.Services;

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
            var sessionIdString = context.User.Claims.FirstOrDefault(claim => claim.Type == "sessionId")?.Value;
            if (Guid.TryParse(sessionIdString, out var sessionId))
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
