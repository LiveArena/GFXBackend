using GraphicsBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraphicsBackend.Controllers
{
    [Route("ws")]
    [ApiController]
    public class WebSocketController : ControllerBase
    {
        private readonly WebSocketService _socketServerService;
        public WebSocketController(WebSocketService socketServerService)
        {
           _socketServerService= socketServerService;    
        }
        [HttpGet]
        public async Task Connect()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _socketServerService.AddSocketAsync(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }
        [HttpPost]
        public async Task SendMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                HttpContext.Response.StatusCode = 400;
                return;
            }

            await _socketServerService.NotifyClientsAsync(message);
        }
    }
}
