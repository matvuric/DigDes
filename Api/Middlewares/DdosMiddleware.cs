using Api.Services;

namespace Api.Middlewares
{
    public class DdosMiddleware
    {
        private readonly RequestDelegate _next;

        public DdosMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, DdosGuard guard)
        {
            var headerAuth = context.Request.Headers.Authorization;

            try
            {
                guard.CheckRequest(headerAuth);
                await _next(context);
            }
            catch (DdosGuard.TooManyRequestsException)
            {
                context.Response.StatusCode = 429;
                await context.Response.WriteAsJsonAsync("Too many requests, allowed 10 requests per second");
            }
            
        }
    }
    public static class DdosMiddlewareExtensions
    {
        public static IApplicationBuilder UserAntiDdosCustom(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DdosMiddleware>();
        }
    }
}