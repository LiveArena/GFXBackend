using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using GraphicsBackend.Notifications;

namespace GraphicsBackend.Services
{
    public class SocketServerService : BackgroundService
    {
        private readonly SocketServer _socketServer;       

        public SocketServerService(SocketServer socketServer)
        {
            _socketServer = socketServer;
           
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _socketServer.StartServer("127.0.0.1", 5000);
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _socketServer.StopServer();
            return base.StopAsync(cancellationToken);
        }

       
       

    }
    public static class WebSocketHandler
    {
        private static readonly ConcurrentBag<WebSocket> _webSockets = new ConcurrentBag<WebSocket>();

        public static async Task HandleWebSocketAsync(WebSocket webSocket)
        {
            _webSockets.Add(webSocket);
            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.CloseStatus.HasValue)
                {
                    _webSockets.TryTake(out _);
                    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                }
            }
        }

        public static async Task NotifyClientsAsync(string message)
        {
            var tasks = new List<Task>();

            foreach (var socket in _webSockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    var messageBuffer = Encoding.UTF8.GetBytes(message);
                    var messageSegment = new ArraySegment<byte>(messageBuffer);
                    tasks.Add(socket.SendAsync(messageSegment, WebSocketMessageType.Text, true, CancellationToken.None));
                }
            }

            await Task.WhenAll(tasks);
        }
    
     }
}
