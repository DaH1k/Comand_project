using ChatClient.Models;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ChatClient.Services
{
    public class ClientService
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private StreamReader? _reader;

        public event Action<MessageModel>? MessageReceived;
        public event Action<List<string>>? UsersListReceived;

        private TaskCompletionSource<string>? _loginResultTcs;
        private TaskCompletionSource<string>? _registerResultTcs;

        public async Task ConnectAsync(string host, int port)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(host, port);

            _stream = _client.GetStream();
            _reader = new StreamReader(_stream, Encoding.UTF8);

            _ = ListenAsync();
        }

        private async Task ListenAsync()
        {
            while (true)
            {
                try
                {
                    if (_reader == null)
                        break;

                    string? json = await _reader.ReadLineAsync();

                    if (json == null)
                        break;

                    var loginResult = JsonSerializer.Deserialize<LoginResultDto>(json);
                    if (loginResult?.Type == "LoginResult")
                    {
                        _loginResultTcs?.TrySetResult(loginResult.Text);
                        continue;
                    }

                    var registerResult = JsonSerializer.Deserialize<RegisterResultDto>(json);
                    if (registerResult?.Type == "RegisterResult")
                    {
                        _registerResultTcs?.TrySetResult(registerResult.Text);
                        continue;
                    }

                    var usersList = JsonSerializer.Deserialize<UsersListDto>(json);
                    if (usersList?.Type == "UsersList")
                    {
                        UsersListReceived?.Invoke(usersList.Users);
                        continue;
                    }

                    var msg = JsonSerializer.Deserialize<MessageModel>(json);
                    if (msg?.Text != null)
                    {
                        MessageReceived?.Invoke(msg);
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Connection closed: {ex.Message}");
                    break;
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON parsing error: {ex.Message}");
                    continue;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                    break;
                }
            }
        }

        public async Task SendRawAsync(string json)
        {
            if (_stream == null)
                return;

            json += "\n";

            byte[] data = Encoding.UTF8.GetBytes(json);
            await _stream.WriteAsync(data, 0, data.Length);
        }

        public Task<string> WaitForLoginResultAsync()
        {
            _loginResultTcs = new TaskCompletionSource<string>();
            return _loginResultTcs.Task;
        }

        public Task<string> WaitForRegisterResultAsync()
        {
            _registerResultTcs = new TaskCompletionSource<string>();
            return _registerResultTcs.Task;
        }

        public async Task SendMessageAsync(MessageModel msg)
        {
            var dto = new
            {
                Type = "Message",
                Text = msg.Text
            };

            string json = JsonSerializer.Serialize(dto);
            await SendRawAsync(json);
        }

        public void Disconnect()
        {
            _stream?.Close();
            _client?.Close();
        }
    }

    public class LoginResultDto
    {
        public string Type { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }

    public class RegisterResultDto
    {
        public string Type { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }

    public class UsersListDto
    {
        public string Type { get; set; } = string.Empty;
        public List<string> Users { get; set; } = new();
    }
}