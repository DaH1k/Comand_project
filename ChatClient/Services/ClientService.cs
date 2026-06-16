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

        public event Action<MessageModel>? MessageReceived;

    public async Task ConnectAsync(string host, int port)
    {
        _client = new TcpClient();
        await _client.ConnectAsync(host, port);
        _stream = _client.GetStream();

        _ = ListenAsync();
    }

        private async Task ListenAsync()
        {
            var buffer = new byte[4096];
            while (true)
            {
                try
                {
                    int bytesRead = await _stream.ReadAsync(buffer);
                    if (bytesRead == 0)
                    {
                        // клієнт відключився
                        break;
                    }

                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Якщо це результат логіну
                    var loginResult = JsonSerializer.Deserialize<LoginResultDto>(json);
                    if (loginResult?.Type == "LoginResult")
                    {
                        _loginResultTcs?.TrySetResult(loginResult.Text);
                        continue;
                    }

                    // Якщо це результат реєстрації
                    var registerResult = JsonSerializer.Deserialize<RegisterResultDto>(json);
                    if (registerResult?.Type == "RegisterResult")
                    {
                        _registerResultTcs?.TrySetResult(registerResult.Text);
                        continue;
                    }

                    // Інакше — звичайне повідомлення
                    var msg = JsonSerializer.Deserialize<MessageModel>(json);
                    if (msg != null)
                        MessageReceived?.Invoke(msg);
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Connection closed: {ex.Message}");
                    break;
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON parsing error: {ex.Message}");
                    // Можна продовжити читання або вийти
                    continue;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                    break;
                }
            }
        }

        // 🔹 Відправка сирого JSON
        public async Task SendRawAsync(string json)
    {
        byte[] data = Encoding.UTF8.GetBytes(json);
        await _stream.WriteAsync(data);
    }

    // 🔹 Очікування результату логіну
    private TaskCompletionSource<string>? _loginResultTcs;

    public Task<string> WaitForLoginResultAsync()
    {
        _loginResultTcs = new TaskCompletionSource<string>();
        return _loginResultTcs.Task;
    }

    // 🔹 Очікування результату реєстрації
    private TaskCompletionSource<string>? _registerResultTcs;

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
        _client.Close();
    }
}

// DTO для результату логіну
public class LoginResultDto
{
    public string Type { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

    public class RegisterResultDto
    {
        public string Type { get; set; } = string.Empty;  // "RegisterResult"
        public string Text { get; set; } = string.Empty;  // "OK" або "FAIL"
    }
}
