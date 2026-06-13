using System.Net;
using System.Net.Sockets;
using ChatServer.Clients;
using ChatServer.Services;

namespace ChatServer.Server;

public class ChatTcpServer
{
    private readonly int _port;
    private readonly List<ClientConnection> _clients = new();
    private TcpListener _listener;

    private readonly MessageRouter _router;

    public ChatTcpServer(int port)
    {
        _port = port;
        _router = new MessageRouter();
    }

    public async Task StartAsync()
    {
        _listener = new TcpListener(IPAddress.Any, _port);
        _listener.Start();

        Console.WriteLine($"Server started on port {_port}");

        while (true)
        {
            var tcpClient = await _listener.AcceptTcpClientAsync();
            Console.WriteLine("Client connected");

            var client = new ClientConnection(tcpClient, this);
            _clients.Add(client);

            _ = client.HandleAsync();
        }
    }

    public void RemoveClient(ClientConnection client)
    {
        _clients.Remove(client);
    }

    public async Task BroadcastAsync(string message, ClientConnection sender)
    {
        foreach (var client in _clients)
        {
            if (client != sender)
            {
                await client.SendAsync(message);
            }
        }
    }
}
