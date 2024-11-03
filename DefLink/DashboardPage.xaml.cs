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
            var profile = GetVpnProfile();
            UpdateConfigFile(profile);

        }

        private VpnProfile GetVpnProfile()
        {
            // Здесь можно получить данные из базы данных и передать их в объект VpnProfile
            return new VpnProfile
            {
                Id = "469e3ad3-cb41-415e-bff6-0604436e89fe", // Обновлённый Id
                Address = "150.241.101.254", // Обновлённый адрес
                Port = 443, // Порт остаётся 443
                Security = "reality" // Метод безопасности
            };
        }

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
                    clients = new[]
                    {
                        new
                        {
                            id = profile.Id,
                            level = 0,
                            email = "user@example.com"
                        }
                    }
                }
            }
        },
                outbounds = new[]
                {
            new
            {
                protocol = "vless", // Здесь важно, чтобы вы использовали vless
                settings = new
                {
                    vnext = new[]
                    {
                        new
                        {
                            address = profile.Address,
                            port = 443,
                            users = new[]
                            {
                                new
                                {
                                    id = profile.Id,
                                    alterId = 0,
                                    security = profile.Security,
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
                    rules = new[]
                    {
                new { type = "field", outboundTag = "proxy", ip = new[] { "geoip:private" } }
            }
                }
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);

            // Вывод для отладки
            Console.WriteLine("Запись конфигурации в файл:");
            Console.WriteLine(json);

            System.IO.File.WriteAllText("xray/config.json", json); // ПРОБЛЕМНАЯ ЧАСТЬ КОДА ТУТ!!!! ОШИБКА И БЛАБЛА

        }


        private async void ConnectToVpnButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(1000); // Задержка перед выполнением

            var profile = GetVpnProfile();
            UpdateConfigFile(profile); // Обновляем конфигурацию перед подключением

            // Путь к xray.exe и config.json
            string xrayPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xray", "xray.exe");
            string configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xray", "config.json");


            if (!isConnected)
            {
                try
                {
                    xrayProcess = Process.Start(new ProcessStartInfo
                    {
                        FileName = xrayPath,
                        Arguments = $"-config \"{configPath}\"",
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

                        // Обработка ошибок
                        xrayProcess.ErrorDataReceived += (s, errorEventArgs) =>
                        {
                            if (!string.IsNullOrEmpty(errorEventArgs.Data))
                            {
                                // Логирование ошибок или вывод информации о процессе
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
                        xrayProcess.WaitForExit(); // Ждём завершения
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

        private void HandleConnectionError(string message)
        {
            ConnectionStatusText.Text = "Статус: Ошибка подключения";
            ConnectionStatusText.Foreground = new SolidColorBrush(Colors.Red);
            MessageBox.Show(message);
        }

        // Метод для перезапуска сетевого адаптера, чтобы восстановить подключение
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
