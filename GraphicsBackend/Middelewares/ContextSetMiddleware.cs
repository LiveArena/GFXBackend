using GraphicsBackend.Contexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GraphicsBackend.Middelewares
{
    public class ContextSetMiddleware
    {
        private readonly RequestDelegate _next;
        public ContextSetMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var accessToken = await context.GetTokenAsync("access_token");
            var contextUser = context.Items["Subject"];
            if (accessToken is not null && contextUser is null)
            {

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(accessToken);
                var sub = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub");
                context.Items.Add("Subject", sub);

            }

            await _next(context);
        }
    }
}
