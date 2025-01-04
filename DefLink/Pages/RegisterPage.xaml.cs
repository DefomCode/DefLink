using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DefLink
{
    public partial class RegisterPage : Page
    {
        private DatabaseManager dbManager;

        public RegisterPage()
        {
            InitializeComponent();
            dbManager = new DatabaseManager("Server=DESKTOP-K3IPTFJ;Database=DefLink;Trusted_Connection=True;"); // Строка подключения
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string login = RegisterLoginTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = RegisterPasswordTextBox.Password.Trim();
            string confirmPassword = ConfirmPasswordTextBox.Password.Trim();

            // Проверка на пустые поля с использованием switch
            switch (true)
            {
                case var _ when string.IsNullOrWhiteSpace(login) || login == "Логин":
                    MessageBox.Show("Введите логин.");
                    return;
                case var _ when string.IsNullOrWhiteSpace(email) || email == "Email":
                    MessageBox.Show("Введите email.");
                    return;
                case var _ when string.IsNullOrWhiteSpace(password) || password == "Пароль":
                    MessageBox.Show("Введите пароль.");
                    return;
                case var _ when password != confirmPassword:
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
            NavigationService.Navigate(new LoginPage());
        }

        private void RegisterLoginTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (RegisterLoginTextBox.Text == "Логин")
            {
                RegisterLoginTextBox.Text = "";
                RegisterLoginTextBox.Foreground = Brushes.Black;
            }
        }

        private void RegisterLoginTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RegisterLoginTextBox.Text))
            {
                RegisterLoginTextBox.Text = "Логин";
                RegisterLoginTextBox.Foreground = Brushes.Gray;
            }
        }

        private void EmailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (EmailTextBox.Text == "Email")
            {
                EmailTextBox.Text = "";
                EmailTextBox.Foreground = Brushes.Black;
            }
        }

        private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                EmailTextBox.Text = "Email";
                EmailTextBox.Foreground = Brushes.Gray;
            }
        }

        private void RegisterPasswordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (RegisterPasswordTextBox.Password == "Пароль")
            {
                RegisterPasswordTextBox.Clear();
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
                ConfirmPasswordTextBox.Clear();
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
