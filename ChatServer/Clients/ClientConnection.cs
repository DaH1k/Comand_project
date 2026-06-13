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

                await _server.BroadcastAsync(json, this);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
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