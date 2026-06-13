namespace ChatServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var server = new ChatTcpServer(5000);
            await server.StartAsync();
        }
    }
}
