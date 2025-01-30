namespace GraphicsBackend
{
    using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private const string API_KEY_HEADER = "X-API-KEY";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var configuration = (IConfiguration)context.HttpContext.RequestServices.GetService(typeof(IConfiguration));

        if (!context.HttpContext.Request.Headers.TryGetValue(API_KEY_HEADER, out var extractedApiKey))
        {
            context.Result = new ContentResult()
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Content = "API Key is missing."
            };
            return;
        }

        var apiKey = configuration.GetValue<string>("ApiKey");

        if (!apiKey.Equals(extractedApiKey))
        {
            context.Result = new ContentResult()
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Content = "Unauthorized client."
            };
            return;
        }
    }
}

}
