using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DefLink
{
    /// <summary>
    /// Логика взаимодействия для DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ваш VLESS профиль
                string vpnUri = "vless://7dff898a-147d-4a4b-83e5-17c9bf21174f@185.225.200.25:443?security=reality&type=tcp&headerType=&path=&host=&sni=www.ign.com&fp=chrome&pbk=J3ALGf6z4yIDgDyuvoURdPISvw3HBCd1xAObATB9y0U&sid=6318d4479bdc45c9#%F0%9F%9A%80%20Marz%20%28admin%29%20%5BVLESS%20-%20tcp%5D";

                var profile = VpnProfile.Parse(vpnUri); // Метод для парсинга вашего VLESS профиля
                ConnectToVpn(profile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
            }
        }

        private void ConnectToVpn(VpnProfile profile)
        {
            // Замените на вашу команду для подключения к VPN-клиенту
            string command = $"your_vpn_client.exe --uuid {profile.UUID} --address {profile.Address} --port {profile.Port}";
            Process.Start("cmd.exe", $"/C {command}");
        }
    }
}
