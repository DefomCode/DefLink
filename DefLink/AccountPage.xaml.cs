using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation; // Убедитесь, что этот using существует

namespace DefLink
{
    /// <summary>
    /// Логика взаимодействия для AccountPage.xaml
    /// </summary>
    public partial class AccountPage : Page
    {
        private DatabaseManager dbManager;

        public AccountPage()
        {
            InitializeComponent();
            dbManager = new DatabaseManager("Server=DESKTOP-K3IPTFJ;Database=DefLink;Trusted_Connection=True;");
            this.Loaded += AccountPage_Loaded; // Подписываемся на событие Loaded
        }

        private void AccountPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Проверяем состояние входа по сохранённым настройкам
            if (!Properties.Settings.Default.IsLoggedIn)
            {
                NavigationService.Navigate(new LoginPage());
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Очищаем данные о пользователе
            Application.Current.Properties["UserId"] = null;
            Application.Current.Properties["UserLogin"] = null;

            // Сбрасываем состояние входа
            Properties.Settings.Default.IsLoggedIn = false;
            Properties.Settings.Default.UserId = 0;
            Properties.Settings.Default.UserLogin = string.Empty;
            Properties.Settings.Default.Save(); // Сохраняем изменения

            // Перенаправляем на страницу входа
            NavigationService.Navigate(new LoginPage());
        }




    }
}
