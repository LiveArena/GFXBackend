using GraphicsBackend.Notifications;
using GraphicsBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraphicsBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public abstract class WebSocketSupportController : ControllerBase
{
    private readonly IWebSocketService _webSocketService;

    protected WebSocketSupportController(IWebSocketService webSocketService)
    {
        _webSocketService = webSocketService;
    }

    protected abstract SocketMessageType SocketMessageType { get; }

    protected async void BroadcastThroughSocket<T>(ActionTaken action, T message)
            where T : notnull
    {
        var mesasge = new SocketMessage(SocketMessageType, action, message);
        await _webSocketService.BroadcastAsync(mesasge);
    }
}
