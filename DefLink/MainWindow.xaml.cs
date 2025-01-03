using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace DefLink
{
    public partial class MainWindow : Window
    {
        private DatabaseManager dbManager;
        private Dictionary<string, Page> cachedPages;

        public MainWindow()
        {
            InitializeComponent();
            dbManager = new DatabaseManager("Server=DESKTOP-K3IPTFJ;Database=DefLink;Trusted_Connection=True;");
            cachedPages = new Dictionary<string, Page>();
            CheckLogin(); // Проверка авторизации
            this.Icon = new BitmapImage(new Uri("pack://application:,,,/Images/DefLink.ico"));
            this.StateChanged += MainWindow_StateChanged;

            // Проверка, если пользователь авторизован
            if (Properties.Settings.Default.IsLoggedIn)
            {
                // Получение данных пользователя
                string uuid = Properties.Settings.Default.UUID;
                string serverAddress = Properties.Settings.Default.ServerAddress;
                string publicKey = Properties.Settings.Default.PublicKey;
                string label = Properties.Settings.Default.Label;

                // Обновляем конфигурацию с полученными данными
                ConfigGenerator.UpdateConfig(uuid, serverAddress, publicKey, label);
            }
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            // Проверка состояния окна
            if (this.WindowState == WindowState.Minimized)
            {
                // Можно добавить логику, если это необходимо
            }
        }

        private void CheckLogin()
        {
            if (Properties.Settings.Default.IsLoggedIn) // Проверяем, вошел ли пользователь
            {
                // Обновляем данные пользователя при запуске
                UpdateUserSettings();

                // Загружаем главную страницу
                NavigateToPage("DashboardPage");
            }
            else
            {
                NavigateToPage("RegisterPage"); // Если не вошел, переходим на страницу регистрации
            }
        }

        private void UpdateUserSettings()
        {
            int ID_User = Properties.Settings.Default.ID_User; // Получаем ID пользователя из настроек

            // Запрашиваем данные пользователя из базы данных
            var userData = dbManager.GetUserDataById(ID_User);

            // Обновляем данные пользователя в настройках
            Properties.Settings.Default.ID_User = userData.ID_User;
            Properties.Settings.Default.Login = userData.Login;
            Properties.Settings.Default.UUID = userData.UUID;
            Properties.Settings.Default.ServerAddress = userData.ServerAddress;
            Properties.Settings.Default.PublicKey = userData.PublicKey;
            Properties.Settings.Default.Label = userData.Label;

            // Сохраняем изменения в настройках
            Properties.Settings.Default.Save();
        }

        private void NavigateToPage(string pageKey)
        {
            if (!cachedPages.ContainsKey(pageKey))
            {
                // Создаем страницу и добавляем в кэш
                switch (pageKey)
                {
                    case "DashboardPage":
                        cachedPages[pageKey] = new DashboardPage();
                        break;
                    case "RegisterPage":
                        cachedPages[pageKey] = new RegisterPage();
                        break;
                    case "SettingsPage":
                        cachedPages[pageKey] = new SettingsPage();
                        break;
                    case "ServersPage":
                        cachedPages[pageKey] = new ServersPage();
                        break;
                    case "AccountPage":
                        cachedPages[pageKey] = new AccountPage();
                        break;
                }
            }

            // Устанавливаем страницу из кэша
            ContentFrame.Navigate(cachedPages[pageKey]);
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e) => NavigateToPage("DashboardPage");

        private void Settings_Click(object sender, RoutedEventArgs e) => NavigateToPage("SettingsPage");

        private void Servers_Click(object sender, RoutedEventArgs e) => NavigateToPage("ServersPage");

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, если пользователь вошел
            if (Properties.Settings.Default.IsLoggedIn)
            {
                // Если пользователь вошел, открываем страницу аккаунта
                NavigateToPage("AccountPage");
            }
            else
            {
                // Если не вошел, открываем страницу регистрации
                NavigateToPage("RegisterPage");
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized; // Минимизируем окно без анимации
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Сохранение всех настроек, включая данные аккаунта
            Properties.Settings.Default.Save(); // Сохраните изменения
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
