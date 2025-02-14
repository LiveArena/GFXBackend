using System.Net.WebSockets;
using System.Text;
using GraphicsBackend.Configurations;
using GraphicsBackend.Contexts;
using GraphicsBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings.GetValue<string>("SecretKey");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
        ValidAudience = jwtSettings.GetValue<string>("Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});


builder.Services.AddSingleton<JwtTokenGenerator>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token like this: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});



builder.Services.AddAuthorization();

builder.Services.AddDbContext<ApplicationDbContext>(options =>

        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                options => options.CommandTimeout(180)));  // Timeout set to 180 seconds));

#region CosmosBD
// Add DbContext
builder.Services.AddDbContext<CosmosDbContext>(options =>

    options.UseCosmos(
        "https://gfz-dev-database.documents.azure.com:443/",
        "lTbf8onOBEDb22sMK4HDjhmHpWyaKskBMbUzyyJPdU0D10NPwWLgwVXvnt9XkZ8X3WJgu4Ae9pwgACDbFHHAKg==",
        "projects"
    ));

builder.Services.AddSingleton<CosmosClient>(options =>
{
    var configurationSection = builder.Configuration.GetSection("CosmosDb");
    return new CosmosClient(configurationSection["Account"], configurationSection["Key"]);

});
builder.Services.AddScoped(typeof(ICosmosDbService<>), typeof(CosmosDbService<>));

#endregion CosmosDB
#region Redis
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(x =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));
#endregion Redis

var app = builder.Build();

app.UseWebSockets();

app.Map("/ws", async (context) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        string clientId = context.Request.Query["clientId"];
        if (string.IsNullOrEmpty(clientId))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Client ID is required.");
            return;
        }
        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await WebSocketHandler.HandleWebSocketAsync(clientId, webSocket);
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
});



var jwtTokenGenerator = app.Services.GetRequiredService<JwtTokenGenerator>();
var token = jwtTokenGenerator.GenerateToken();

Console.WriteLine($"Generated JWT Token: {token}");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}



app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket)
{
    var buffer = new byte[1024 * 4];
    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

    // Simulate sending data at any time (e.g., every 5 seconds)
    var sendPeriodicUpdatesTask = Task.Run(async () =>
    {
        while (!result.CloseStatus.HasValue)
        {
            var message = $"Server message at {DateTime.UtcNow}";
            var encodedMessage = Encoding.UTF8.GetBytes(message);

            // Send the message to the client
            await webSocket.SendAsync(new ArraySegment<byte>(encodedMessage), WebSocketMessageType.Text, true, CancellationToken.None);

            await Task.Delay(5000); // Wait for 5 seconds before sending the next message
        }
    });

    // Continue listening for client messages
    while (!result.CloseStatus.HasValue)
    {
        await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    }

    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

}
