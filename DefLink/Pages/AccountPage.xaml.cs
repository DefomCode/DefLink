﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            LoadUserData();
        }

        private void LoadUserData()
        {
            try
            {
                // Получаем ID пользователя из настроек
                int userId = Properties.Settings.Default.ID_User;

                // Проверка на случай, если ID_User не установлен (например, 0 или не найден)
                if (userId == 0)
                {
                    UserNameTextBlock.Text = "Пользователь не найден";
                    return;
                }

                // Получаем данные пользователя из базы
                UserData userData = dbManager.GetUserDataById(userId);

                // Проверяем, если данные пользователя не найдены
                if (userData != null)
                {
                    // Отображаем логин пользователя
                    UserNameTextBlock.Text = userData.Login;
                }
                else
                {
                    UserNameTextBlock.Text = "Ошибка загрузки данных";
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                Console.WriteLine("Ошибка: " + ex.Message);
                UserNameTextBlock.Text = "Ошибка при загрузке данных";
            }
        }




        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Очищаем данные о пользователе
            Application.Current.Properties["ID_User"] = null;
            Application.Current.Properties["Login"] = null;

            // Сбрасываем настройки аккаунта
            Properties.Settings.Default.IsLoggedIn = false;
            Properties.Settings.Default.ID_User = 0;
            Properties.Settings.Default.Login = string.Empty;
            Properties.Settings.Default.UUID = string.Empty; // Очистка UUID
            Properties.Settings.Default.ServerAddress = string.Empty; // Очистка ServerAddress
            Properties.Settings.Default.PublicKey = string.Empty; // Очистка PublicKey
            Properties.Settings.Default.Label = string.Empty; // Очистка Label
            Properties.Settings.Default.Save(); // Сохраняем изменения

            // Перенаправляем на страницу входа
            NavigationService.Navigate(new LoginPage());
        }

    }
}
