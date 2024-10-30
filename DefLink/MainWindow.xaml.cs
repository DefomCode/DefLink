using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DefLink
{
    public partial class MainWindow : Window
    {
        private DatabaseManager dbManager;

        public MainWindow()
        {
            InitializeComponent();
            dbManager = new DatabaseManager("Server=DESKTOP-K3IPTFJ;Database=DefLink;Trusted_Connection=True;");
            CheckUserLogin(); // Проверка авторизации
            this.Icon = new BitmapImage(new Uri("pack://application:,,,/Images/DefLink.ico"));
            this.StateChanged += MainWindow_StateChanged;

        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            // Проверка состояния окна
            if (this.WindowState == WindowState.Minimized)
            {
                // Можно добавить логику, если это необходимо
            }
        }


        private void CheckUserLogin()
        {
            if (dbManager.IsUserLoggedIn())
            {
                LoadDashboard();
            }
            else
            {
                ContentFrame.Navigate(new RegisterPage());
            }
        }

        private void LoadDashboard()
        {
            ContentFrame.Navigate(new DashboardPage());
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e) => LoadDashboard();

        private void Settings_Click(object sender, RoutedEventArgs e) => ContentFrame.Navigate(new SettingsPage());

        private void Servers_Click(object sender, RoutedEventArgs e) => ContentFrame.Navigate(new ServersPage());

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            if (dbManager.IsUserLoggedIn())
            {
                ContentFrame.Navigate(new AccountPage());
            }
            else
            {
                ContentFrame.Navigate(new RegisterPage());
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized; // Минимизируем окно без анимации
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Завершение приложения
        }

        private void HeaderGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove(); // Движение окна
            }
        }
    }
}
