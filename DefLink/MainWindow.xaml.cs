using System.Windows;

namespace DefLink
{
    public partial class MainWindow : Window
    {
        private DatabaseManager dbManager;

        public MainWindow()
        {
            InitializeComponent();
            dbManager = new DatabaseManager("YourConnectionString"); // Укажите вашу строку подключения
            CheckUserLogin();
        }

        private void CheckUserLogin()
        {
            if (dbManager.IsUserLoggedIn())
            {
                // Если пользователь вошел, открываем главную страницу
                LoadDashboard();
            }
            else
            {
                // Если пользователь не вошел, открываем страницу регистрации
                ContentFrame.Navigate(new RegisterPage());
            }
        }

        private void LoadDashboard()
        {
            // Загружаем DashboardPage в Frame
            ContentFrame.Navigate(new DashboardPage());
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            LoadDashboard();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            // Загрузка страницы настроек
            ContentFrame.Navigate(new SettingsPage());
        }

        private void Servers_Click(object sender, RoutedEventArgs e)
        {
            // Загрузка страницы серверов
            ContentFrame.Navigate(new ServersPage());
        }

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            // Проверка входа пользователя перед переходом на страницу аккаунта
            if (dbManager.IsUserLoggedIn())
            {
                // Если пользователь вошел, загружаем страницу аккаунта
                ContentFrame.Navigate(new AccountPage());
            }
            else
            {
                // Если пользователь не вошел, перенаправляем на страницу регистрации
                ContentFrame.Navigate(new RegisterPage());
            }
        }

    }
}
