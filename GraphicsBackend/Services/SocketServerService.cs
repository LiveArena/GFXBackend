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
        private static readonly Dictionary<string, WebSocket> _connections = new();

        public static async Task HandleWebSocketAsync(string clientId, WebSocket webSocket)
        {
            _connections[clientId] = webSocket;
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;
            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        _connections.Remove(clientId);
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    }
                    else
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"Received from {clientId}: {message}");
                        // Handle messages here
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");
            }
            finally
            {
                if (_connections.ContainsKey(clientId))
                    _connections.Remove(clientId);

                webSocket.Dispose();
            }
        }

        public static async Task NotifyClientsAsync(string message, string clientId)
        {
            if (_connections.TryGetValue(clientId, out WebSocket socket) && socket.State == WebSocketState.Open)
            {
                var messageBuffer = Encoding.UTF8.GetBytes(message);
                var messageSegment = new ArraySegment<byte>(messageBuffer);

                try
                {
                    await socket.SendAsync(messageSegment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending message to {clientId}: {ex.Message}");
                    _connections.Remove(clientId); // Remove disconnected client
                }
            }
            else
            {
                Console.WriteLine($"Client {clientId} not found or connection is closed.");
            }
        }

    }
}
