using ChatClient.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ChatClient.Views
{
    public partial class ChatView : UserControl
    {
        private readonly MainViewModel _viewModel;

        public ChatView(MainViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = _viewModel;

            Loaded += ChatView_Loaded;
        }

        private void ChatView_Loaded(object sender, RoutedEventArgs e)
        {
            MessagesList.ItemsSource = _viewModel.Messages;
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            string text = MessageBox.Text.Trim();

            if (!string.IsNullOrEmpty(text))
            {
                await _viewModel.SendMessageAsync(text);
                MessageBox.Clear();
            }
        }
    }
}