using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Concurrent;

namespace GraphicsBackend.Notifications
{
    public class SocketServer
    {
        private TcpListener? _listener;
        private readonly ConcurrentBag<TcpClient> _clients = new();

        public void StartServer(string ipAddress, int port)
        {
            _listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            _listener.Start();
            Console.WriteLine($"Server started on {ipAddress}:{port}");

            Task.Run(ListenForClients);
        }

        private async Task ListenForClients()
        {
            while (_listener != null)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
                _clients.Add(client);  // Track the connected clients
                Console.WriteLine("Client connected.");

                Task.Run(() => HandleClient(client));
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            using NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length))) != 0)
            {
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {receivedMessage}");

                // Echo or process the message received from the client
            }

            client.Close();
            Console.WriteLine("Client disconnected.");
        }

        // New method to send messages to all connected clients
        public async Task SendMessageToAllClients(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            foreach (var client in _clients)
            {
                if (client.Connected)
                {
                    try
                    {
                        await client.GetStream().WriteAsync(messageBytes.AsMemory(0, messageBytes.Length));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending message to client: {ex.Message}");
                    }
                }
            }
        }

        public void StopServer()
        {
            _listener?.Stop();
            Console.WriteLine("Server stopped.");
        }
    }
}
