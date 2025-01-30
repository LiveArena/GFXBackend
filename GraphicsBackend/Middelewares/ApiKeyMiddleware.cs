namespace GraphicsBackend.Middelewares
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;

    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string API_KEY_HEADER = "X-API-KEY";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out var extractedApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API Key is missing.");
                return;
            }

            var apiKey = configuration["ApiKey"];

            if (!apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized client.");
                return;
            }

            await _next(context);
        }
    }

}
