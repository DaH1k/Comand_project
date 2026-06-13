using System.Net.Sockets;
using System.Text;
using ChatServer.Server;

namespace ChatServer.Clients;

public class ClientConnection
{
    private readonly TcpClient _client;
    private readonly ChatTcpServer _server;
    private NetworkStream _stream;

    public string Username { get; set; }

    public ClientConnection(TcpClient client, ChatTcpServer server)
    {
        _client = client;
        _server = server;
        _stream = client.GetStream();
    }

    public async Task HandleAsync()
{
    try
    {
        byte[] buffer = new byte[4096];

        while (true)
        {
            int bytesRead = await _stream.ReadAsync(buffer);

            if (bytesRead == 0)
                break;

            string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            Console.WriteLine($"Received: {json}");

            var message = JsonSerializer.Deserialize<MessageDto>(json);

            if (message == null)
                continue;

            // LOGIN
            if (message.Type == "Login")
            {
                Username = message.Sender;

                Console.WriteLine($"{Username} is ONLINE");

                await _server.BroadcastSystemAsync($"{Username} joined the chat");
                continue;
            }

            // MESSAGE
            if (message.Type == "Message")
            {
                await _server.BroadcastAsync(json, this);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
    finally
    {
        if (!string.IsNullOrEmpty(Username))
        {
            Console.WriteLine($"{Username} is OFFLINE");
            await _server.BroadcastSystemAsync($"{Username} left the chat");
        }

        _server.RemoveClient(this);
        _client.Close();
    }
}

    public async Task SendAsync(string message)
    {
        if (_stream.CanWrite)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            await _stream.WriteAsync(data);
        }
    }
}
