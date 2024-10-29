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
    public partial class RegisterPage : Page
    {
        private DatabaseManager dbManager;

        public RegisterPage()
        {
            InitializeComponent();
            dbManager = new DatabaseManager("YourConnectionString"); // Укажите вашу строку подключения
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string login = RegisterLoginTextBox.Text; // Используйте правильное имя
            string email = EmailTextBox.Text; // Правильное имя
            string password = RegisterPasswordTextBox.Password;

            // Проверка паролей на совпадение
            if (password != ConfirmPasswordTextBox.Password)
            {
                MessageBox.Show("Пароли не совпадают.");
                return;
            }

            if (dbManager.RegisterUser(login, email, password))
            {
                MessageBox.Show("Регистрация прошла успешно. Вы можете войти в систему.");
                NavigationService.Navigate(new LoginPage());
            }
            else
            {
                MessageBox.Show("Ошибка регистрации. Попробуйте снова.");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Возвращаемся на страницу входа
            NavigationService.Navigate(new LoginPage());
        }

        private void RegisterLoginTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (RegisterLoginTextBox.Text == "Логин")
            {
                RegisterLoginTextBox.Text = "";
                RegisterLoginTextBox.Foreground = Brushes.Black; // Цвет текста
            }
        }

        private void RegisterLoginTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RegisterLoginTextBox.Text))
            {
                RegisterLoginTextBox.Text = "Логин";
                RegisterLoginTextBox.Foreground = Brushes.Gray; // Цвет заполнителя
            }
        }

        private void EmailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (EmailTextBox.Text == "Email")
            {
                EmailTextBox.Text = "";
                EmailTextBox.Foreground = Brushes.Black; // Цвет текста
            }
        }

        private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                EmailTextBox.Text = "Email";
                EmailTextBox.Foreground = Brushes.Gray; // Цвет заполнителя
            }
        }

        private void RegisterPasswordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (RegisterPasswordTextBox.Password == "Пароль")
            {
                RegisterPasswordTextBox.Clear(); // Очистка пароля
            }
        }

        private void RegisterPasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RegisterPasswordTextBox.Password))
            {
                RegisterPasswordTextBox.Password = "Пароль";
            }
        }

        private void ConfirmPasswordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ConfirmPasswordTextBox.Password == "Подтвердите пароль")
            {
                ConfirmPasswordTextBox.Clear(); // Очистка пароля
            }
        }

        private void ConfirmPasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ConfirmPasswordTextBox.Password))
            {
                ConfirmPasswordTextBox.Password = "Подтвердите пароль";
            }
        }
    }
}
