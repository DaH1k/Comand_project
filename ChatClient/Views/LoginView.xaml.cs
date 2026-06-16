using ChatClient;
using ChatClient.Services;
using ChatClient.ViewModels;
using System.Text.Json;
using System.Windows;

namespace ChatClient.Views
{
    public partial class LoginView : Window
    {
        private readonly ClientService _clientService = new ClientService();

        public LoginView()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;

            // 🔹 Перевірка на пусті поля
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Будь ласка, введіть логін і пароль");
                return;
            }

            try
            {
                // 🔹 Підключення до сервера (адреса і порт підстав свої)
                await _clientService.ConnectAsync("127.0.0.1", 8888);

                // 🔹 Формуємо DTO для логіну
                var dto = new { Type = "Login", Login = login, Password = password };
                string json = JsonSerializer.Serialize(dto);

                // 🔹 Відправляємо на сервер
                var waitResult = _clientService.WaitForLoginResultAsync();

                await _clientService.SendRawAsync(json);

                string result = await waitResult;

                if (result == "OK")
                {
                    var vm = new MainViewModel(_clientService);
                    vm.Username = login;

                    var main = new MainWindow(vm);
                    main.Show();

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Невірний логін або пароль");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при логіні: {ex.Message}");
            }
        }


        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirm = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirm))
            {
                MessageBox.Show("Будь ласка, заповніть усі поля");
                return;
            }
            if (password != confirm)
            {
                MessageBox.Show("Паролі не співпадають");
                return;
            }

            try
            {
                await _clientService.ConnectAsync("127.0.0.1", 8888);

                var dto = new { Type = "Register", Login = login, Password = password };
                string json = JsonSerializer.Serialize(dto);

                var waitResult = _clientService.WaitForRegisterResultAsync();

                await _clientService.SendRawAsync(json);

                string result = await waitResult;
                if (result == "OK")
                    MessageBox.Show("Реєстрація успішна!");
                else
                    MessageBox.Show("Такий логін вже існує");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при реєстрації: {ex.Message}");
            }
        }
    }
}
