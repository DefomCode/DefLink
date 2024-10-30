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
            if (!dbManager.IsUserLoggedIn())
            {
                NavigationService.Navigate(new LoginPage());
            }
        }
    }
}
