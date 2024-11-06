using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading.Tasks; // Не забудьте добавить этот using, если его еще нет

namespace DefLink
{
    public partial class DashboardPage : Page
    {
        private bool isConnected = false; // Переменная состояния подключения
        private Process xrayProcess; // Для хранения ссылки на процесс xray

        public DashboardPage()
        {
            InitializeComponent();
            // Обновление конфигурации при инициализации страницы
            var ID_User = GetCurrentUserId(); // Получаем ID текущего пользователя
            var profile = GetVpnProfile(ID_User); // Получаем профиль для текущего пользователя
            UpdateConfigFile(profile); // Обновляем конфигурацию
        }

        // Метод для получения ID текущего пользователя (например, из базы данных или текущей сессии)
        private string GetCurrentUserId()
        {
            // Возвращаем пример ID пользователя
            // Реализуйте этот метод по вашему усмотрению (например, извлекая ID из базы данных или сессии)
            return "exampleID_User";
        }

        // Получаем профиль для пользователя по его ID
        private VpnProfile GetVpnProfile(string ID_User)
        {
            // Реализуйте логику для получения профиля по ID_User 
            // Пример возврата объекта с нужными данными
            return new VpnProfile
            {
                UUID = "some-uuid",  // Замените на актуальные данные
                Label = "MyProfile",
                ServerAdress = "127.0.0.1", // Укажите адрес сервера
                PublicKey = "public-key" // Укажите публичный ключ
            };
        }

        // Метод для обновления конфигурационного файла
        private void UpdateConfigFile(VpnProfile profile)
        {
            var config = new
            {
                inbounds = new[] // Обратите внимание, что здесь можно добавить свои настройки
                {
                    new
                    {
                        port = 443,
                        listen = "0.0.0.0",
                        protocol = "vless",
                        settings = new
                        {
                            clients = new[] { new { id = profile.UUID, level = 0, email = "user@example.com", label = profile.Label } }
                        }
                    }
                },
                outbounds = new[] {
                    new
                    {
                        protocol = "vless",
                        settings = new
                        {
                            vnext = new[] {
                                new
                                {
                                    address = profile.ServerAdress,
                                    port = 443, // Можно добавить динамическую настройку порта
                                    users = new[] {
                                        new {
                                            id = profile.UUID,
                                            alterId = 0,
                                            security = profile.PublicKey,
                                            fingerprint = "random"
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                routing = new
                {
                    rules = new[] { new { type = "field", outboundTag = "proxy", ip = new[] { "geoip:private" } } }
                }
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);

            // Путь для записи конфигурации
            string xrayDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xray");
            if (!System.IO.Directory.Exists(xrayDirectory)) // Проверяем, существует ли папка
            {
                System.IO.Directory.CreateDirectory(xrayDirectory); // Создаем папку, если она не существует
            }

            string configFilePath = System.IO.Path.Combine(xrayDirectory, "config.json");
            System.IO.File.WriteAllText(configFilePath, json); // Запись конфигурации в файл

            // Вывод для отладки
            Console.WriteLine("Запись конфигурации в файл:");
            Console.WriteLine(json);
        }

        // Обработчик для подключения и отключения VPN
        private async void ConnectToVpnButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем ID текущего пользователя
            var ID_User = GetCurrentUserId();

            // Получаем профиль для текущего пользователя
            var profile = GetVpnProfile(ID_User);
            UpdateConfigFile(profile); // Обновляем конфигурацию перед подключением

            // Путь к xray.exe и config.json
            string xrayPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xray", "xray.exe");
            string configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xray", "config.json");

            if (!isConnected)
            {
                try
                {
                    // Проверяем, существует ли файл xray.exe
                    if (!System.IO.File.Exists(xrayPath))
                    {
                        HandleConnectionError($"Не найден файл xray.exe по пути: {xrayPath}");
                        return;
                    }

                    // Запуск процесса xray
                    xrayProcess = Process.Start(new ProcessStartInfo
                    {
                        FileName = xrayPath,
                        Arguments = $"-config \"{configPath}\"", // Указываем конфиг для запуска
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    });

                    // Ждем некоторое время для запуска процесса
                    await Task.Delay(2000);

                    // Проверяем, запущен ли процесс
                    if (xrayProcess != null && !xrayProcess.HasExited)
                    {
                        isConnected = true;
                        ConnectionStatusText.Text = "Статус: Подключено";
                        ConnectionStatusText.Foreground = new SolidColorBrush(Colors.Green);

                        // Логирование ошибок
                        xrayProcess.ErrorDataReceived += (s, errorEventArgs) =>
                        {
                            if (!string.IsNullOrEmpty(errorEventArgs.Data))
                            {
                                // Логирование ошибок
                                Console.WriteLine(errorEventArgs.Data);
                            }
                        };

                        xrayProcess.BeginErrorReadLine(); // Начинаем чтение ошибок
                    }
                    else
                    {
                        HandleConnectionError("Процесс VPN не смог запуститься.");
                    }
                }
                catch (Exception ex)
                {
                    HandleConnectionError($"Не удалось подключиться к VPN: {ex.Message}");
                }
            }
            else
            {
                // Отключаемся
                try
                {
                    if (xrayProcess != null && !xrayProcess.HasExited)
                    {
                        xrayProcess.Kill(); // Завершаем процесс
                        xrayProcess.WaitForExit(); // Ждем завершения
                        xrayProcess.Dispose(); // Освобождаем ресурсы
                    }

                    isConnected = false;
                    ConnectionStatusText.Text = "Статус: Отключено";
                    ConnectionStatusText.Foreground = new SolidColorBrush(Colors.Gray);
                    ResetNetworkAdapter(); // Пробуем перезагрузить сетевой интерфейс
                }
                catch (Exception ex)
                {
                    HandleConnectionError($"Не удалось отключиться от VPN: {ex.Message}");
                }
            }
        }

        // Метод для обработки ошибок подключения
        private void HandleConnectionError(string message)
        {
            ConnectionStatusText.Text = "Статус: Ошибка подключения";
            ConnectionStatusText.Foreground = new SolidColorBrush(Colors.Red);
            MessageBox.Show(message);
            Console.WriteLine(message); // Добавление вывода ошибки в консоль
        }

        // Метод для перезапуска сетевого адаптера
        private void ResetNetworkAdapter()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/C ipconfig /release & ipconfig /renew",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            }).WaitForExit();
        }
    }
}
