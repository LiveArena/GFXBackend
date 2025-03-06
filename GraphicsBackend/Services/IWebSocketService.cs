using System.Net.WebSockets;
using GraphicsBackend.Notifications;

namespace GraphicsBackend.Services
{
    public interface IWebSocketService
    {
        Task AddSocketAsync(WebSocket webSocket);
        Task BroadcastAsync(SocketMessage message);
        Task NotifyClientsAsync(string message);
    }
}