using System.Net.WebSockets;
using System.Text;

namespace GraphicsBackend.Services
{
    public static class WebSocketHandler
    {
        private static readonly Dictionary<string, WebSocket> _connections = new();

        public static async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket)
        {
            
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
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


    }
}
