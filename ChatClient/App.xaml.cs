using System.Windows;
using ChatClient.Views;

namespace ChatClient
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            LoginView loginView = new LoginView();
            loginView.Show();
        }
    }
}
