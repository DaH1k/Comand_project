using ChatClient.Services;
using System.Text.Json;
using System.Windows;

namespace ChatClient.Views
{
    public partial class RegisterView : Window
    {
        private readonly ClientService _clientService = new ClientService();

        public RegisterView()
        {
            InitializeComponent();
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirm = ConfirmBox.Password;

            // Перевірка на пусті поля
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirm))
            {
                MessageBox.Show("Будь ласка, заповніть усі поля");
                return;
            }

            // Перевірка збігу паролів
            if (password != confirm)
            {
                MessageBox.Show("Паролі не співпадають");
                return;
            }

            // Формуємо DTO для реєстрації
            var dto = new { Type = "Register", Login = login, Password = password };
            string json = JsonSerializer.Serialize(dto);

            await _clientService.SendRawAsync(json);
            string result = await _clientService.WaitForRegisterResultAsync();

            if (result == "OK")
            {
                MessageBox.Show("Акаунт успішно створено!");
                new LoginView().Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Такий логін вже існує або помилка реєстрації");
            }
        }
    }
}
