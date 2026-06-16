using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChatClient.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void ApplyTheme_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow == null) return;

            if (ThemeComboBox.SelectedItem is not ComboBoxItem selectedItem) return;
            string theme = selectedItem.Content.ToString();

            string mainBg = "#181824";
            string menuBg = "#11111A";

            if (theme == "Light")
            {
                mainBg = "#F5F5FA";
                menuBg = "#E5E5F0";
            }
            else if (theme == "Blue")
            {
                mainBg = "#1A2634";
                menuBg = "#111A24";
            }

            var mainBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(mainBg);
            var menuBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(menuBg);

            // 4. Фарбуємо головне вікно
            mainWindow.Background = mainBrush;

            // 5. Автоматично шукаємо ліве меню всередині вікна та фарбуємо його
            if (mainWindow.Content is Grid mainGrid)
            {
                foreach (var child in mainGrid.Children)
                {
                    // Якщо елемент знаходиться в першій колонці (Column 0) — це наше меню
                    if (child is Grid leftMenu && Grid.GetColumn(leftMenu) == 0)
                    {
                        leftMenu.Background = menuBrush;
                        break;
                    }
                }
            }
        }
    }
}
