using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading.Tasks;
using Newtonsoft.Json; // Добавьте для работы с JSON

namespace DefLink
{
    public partial class DashboardPage : Page
    {
        private bool isConnected = false; // Переменная состояния подключения
        private Process xrayProcess; // Для хранения ссылки на процесс xray

        public DashboardPage()
        {
            InitializeComponent();

            // Проверка, если пользователь авторизован
            if (Properties.Settings.Default.IsLoggedIn)
            {
                // Получаем данные пользователя из настроек
                string uuid = Properties.Settings.Default.UUID;
                string serverAddress = Properties.Settings.Default.ServerAddress;
                string publicKey = Properties.Settings.Default.PublicKey;
                string label = Properties.Settings.Default.Label;

                // Обновляем конфигурацию с полученными данными
                UpdateConfig(uuid, serverAddress, publicKey, label);
            }
        }

        // Метод для обновления конфигурации
        private void UpdateConfig(string uuid, string serverAddress, string publicKey, string label)
        {
            var config = new
            {
                inbounds = new[] {
                    new
                    {
                        port = 443,
                        listen = "0.0.0.0",
                        protocol = "vless",
                        settings = new
                        {
                            clients = new[] {
                                new { id = uuid, level = 0, email = "user@example.com", label = label }
                            }
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
                                    address = serverAddress,
                                    port = 443,
                                    users = new[] {
                                        new {
                                            id = uuid,
                                            alterId = 0,
                                            security = publicKey,
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
                    rules = new[] {
                        new { type = "field", outboundTag = "proxy", ip = new[] { "geoip:private" } }
                    }
                }
            };

            string json = JsonConvert.SerializeObject(config, Formatting.Indented);

            // Путь для записи конфигурации
            string xrayDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xray");
            if (!System.IO.Directory.Exists(xrayDirectory))
            {
                System.IO.Directory.CreateDirectory(xrayDirectory);
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
            if (!isConnected)
            {
                // Получаем данные для конфигурации
                string uuid = Properties.Settings.Default.UUID;
                string serverAddress = Properties.Settings.Default.ServerAddress;
                string publicKey = Properties.Settings.Default.PublicKey;
                string label = Properties.Settings.Default.Label;

                // Обновляем конфигурацию перед подключением
                UpdateConfig(uuid, serverAddress, publicKey, label);

                // Путь к xray.exe и config.json
                string xrayPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xray", "xray.exe");
                string configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xray", "config.json");

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
