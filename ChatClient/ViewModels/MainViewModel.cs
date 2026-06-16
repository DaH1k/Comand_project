using ChatClient.Models;
using ChatClient.Services;
using System.Collections.ObjectModel;

namespace ChatClient.ViewModels
{
    public class MainViewModel
    {
        public string Username { get; set; } = "";

        private readonly ClientService _clientService;

        public ObservableCollection<MessageModel> Messages { get; } = new();

        public MainViewModel(ClientService clientService)
        {
            _clientService = clientService;

            _clientService.MessageReceived += msg =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    Messages.Add(msg);
                });
            };
        }

        public async Task SendMessageAsync(string text)
        {
            var msg = new MessageModel
            {
                Sender = Username,
                Text = text
            };

            await _clientService.SendMessageAsync(msg);

            Messages.Add(msg);
        }

        public void Disconnect() => _clientService.Disconnect();
    }
}