using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace DefLink
{
    public partial class LoginPage : Page
    {
        private DatabaseManager dbManager;

        public LoginPage()
        {
            InitializeComponent();
            dbManager = new DatabaseManager("YourConnectionString"); // Укажите вашу строку подключения
        }

        private void LoginTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (LoginTextBox.Text == "Логин")
            {
                LoginTextBox.Text = "";
                LoginTextBox.Foreground = Brushes.Black; // Цвет текста
            }
        }

        private void LoginTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginTextBox.Text))
            {
                LoginTextBox.Text = "Логин";
                LoginTextBox.Foreground = Brushes.Gray; // Цвет заполнителя
            }
        }

        private void PasswordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordTextBox.Password == "Пароль")
            {
                PasswordTextBox.Clear(); // Очистка пароля
            }
        }

        private void PasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PasswordTextBox.Password))
            {
                PasswordTextBox.Password = "Пароль";
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Password;

            if (dbManager.CheckLogin(login, password))
            {
                // Сохраняем состояние входа
                Application.Current.Properties["UserId"] = login; // Сохраните ID аккаунта или другую информацию

                // Переход на главную страницу
                NavigationService.Navigate(new DashboardPage());
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу регистрации
            NavigationService.Navigate(new RegisterPage());
        }
    }
}
