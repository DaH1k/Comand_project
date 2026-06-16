using ChatClient.ViewModels;
using ChatClient.Views;
using System.Windows;

namespace ChatClient
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _mainViewModel;

        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();

            _mainViewModel = mainViewModel;

            MainContent.Content = new ChatView(_mainViewModel);
        }

        private void Chat_Click(object sender, RoutedEventArgs e)
            => MainContent.Content = new ChatView(_mainViewModel);

        private void Profile_Click(object sender, RoutedEventArgs e)
            => MainContent.Content = new ProfileView();

        private void Settings_Click(object sender, RoutedEventArgs e)
            => MainContent.Content = new SettingsView();

        private void Admin_Click(object sender, RoutedEventArgs e)
            => MainContent.Content = new AdminView();

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.Disconnect();

            new LoginView().Show();
            Close();
        }
    }
}