using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using GraphicsBackend.Notifications;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace GraphicsBackend.Services
{
    public  class WebSocketService
    {
        private static readonly List< WebSocket> _connections = new();
        public  async Task AddSocketAsync(WebSocket webSocket)
        {
            _connections.Add(webSocket);
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _connections.Remove(webSocket);
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                }
            }

        }
        public  async Task NotifyClientsAsync(string message)
        {
            var tasks = _connections
            .Where(s => s.State == WebSocketState.Open)
            .Select(s => s.SendAsync(
                new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)),
                WebSocketMessageType.Text, true, CancellationToken.None));

            await Task.WhenAll(tasks);
        }

        public async Task BroadcastAsync(SocketMessage message)
        {
            var serializedMessage = JsonConvert.SerializeObject(message);
            await NotifyClientsAsync(serializedMessage);
        }
    }
}
