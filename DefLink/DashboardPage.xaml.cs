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
                string sid = Properties.Settings.Default.sid;

                // Обновляем конфигурацию с полученными данными
                UpdateConfig(uuid, serverAddress, publicKey, label, sid);
            }
        }

        // Метод для обновления конфигурации
        private void UpdateConfig(string uuid, string serverAddress, string publicKey, string label, string sid)
        {
            var config = new
            {
                log = new
                {
                    loglevel = "debug" // Уровень логирования
                },
                inbounds = new[] {
                    new
                    {
                        port = 1080,
                        listen = "127.0.0.1",
                        protocol = "socks",
                        settings = new
                        {
                            udp = true
                        },
                        sniffing = new
                        {
                            enabled = true,
                            destOverride = new[] { "http", "tls", "quic" },
                            routeOnly = true
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
                                        new
                                        {
                                            id = uuid,
                                            encryption = "none"
                                        }
                                    }
                                }
                            }
                        },
                        streamSettings = new
                        {
                            network = "tcp",
                            security = "reality",
                            realitySettings = new
                            {
                                fingerprint = "random", // Пример фингерпринта
                                serverName = "www.ign.com", // Имя сервера
                                publicKey = publicKey, // Публичный ключ
                                spiderX = "/", // Пример spiderX
                                shortId = sid
                            }
                        },
                        tag = "proxy"
                    }
                }
            };

            string json = JsonConvert.SerializeObject(config, Formatting.Indented);

            string xrayDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xray");
            if (!System.IO.Directory.Exists(xrayDirectory))
            {
                System.IO.Directory.CreateDirectory(xrayDirectory);
            }

            string configFilePath = System.IO.Path.Combine(xrayDirectory, "config.json");
            System.IO.File.WriteAllText(configFilePath, json); // Запись конфигурации в файл
            Console.WriteLine("Запись конфигурации в файл:");
            Console.WriteLine(json);
        }

        // Обработчик для подключения и отключения VPN
        private async void ConnectToVpnButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                string xrayPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xray", "xray.exe");
                string configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xray", "config.json");

                try
                {
                    if (!System.IO.File.Exists(xrayPath))
                    {
                        await HandleConnectionErrorAsync($"Не найден файл xray.exe по пути: {xrayPath}");
                        return;
                    }

                    var startInfo = new ProcessStartInfo
                    {
                        FileName = xrayPath,
                        Arguments = $"-config \"{configPath}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };

                    xrayProcess = new Process { StartInfo = startInfo };

                    // Подписка на события для асинхронного чтения StandardOutput и StandardError
                    var outputTask = new TaskCompletionSource<string>();
                    var errorTask = new TaskCompletionSource<string>();

                    xrayProcess.OutputDataReceived += (s, outputEventArgs) =>
                    {
                        if (outputEventArgs.Data != null)
                            Console.WriteLine("Output: " + outputEventArgs.Data);
                        else
                            outputTask.TrySetResult(null); // Завершение потока вывода
                    };

                    xrayProcess.ErrorDataReceived += (s, errorEventArgs) =>
                    {
                        if (errorEventArgs.Data != null)
                            Console.WriteLine("Error: " + errorEventArgs.Data);
                        else
                            errorTask.TrySetResult(null); // Завершение потока ошибок
                    };

                    xrayProcess.Start();
                    xrayProcess.BeginOutputReadLine();
                    xrayProcess.BeginErrorReadLine();

                    await Task.WhenAll(outputTask.Task, errorTask.Task).ConfigureAwait(false); // Ожидание завершения обоих потоков

                    await Task.Delay(2000); // Задержка для старта процесса

                    if (xrayProcess != null && !xrayProcess.HasExited)
                    {
                        isConnected = true;
                        ConnectionStatusText.Text = "Статус: Подключено";
                        ConnectionStatusText.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        await HandleConnectionErrorAsync("Процесс VPN не смог запуститься.");
                    }
                }
                catch (Exception ex)
                {
                    await HandleConnectionErrorAsync($"Не удалось подключиться к VPN: {ex.Message}");
                }
            }
            else
            {
                try
                {
                    if (xrayProcess != null && !xrayProcess.HasExited)
                    {
                        xrayProcess.Kill();
                        xrayProcess.WaitForExit();
                        xrayProcess.Dispose();
                    }

                    isConnected = false;
                    ConnectionStatusText.Text = "Статус: Отключено";
                    ConnectionStatusText.Foreground = new SolidColorBrush(Colors.Gray);
                    ResetNetworkAdapter();
                }
                catch (Exception ex)
                {
                   await HandleConnectionErrorAsync($"Не удалось отключиться от VPN: {ex.Message}");
                }
            }
        }

        // Асинхронный метод для обработки ошибок подключения
        private async Task HandleConnectionErrorAsync(string message)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                ConnectionStatusText.Text = "Статус: Ошибка подключения";
                ConnectionStatusText.Foreground = new SolidColorBrush(Colors.Red);
                MessageBox.Show(message);
            });
            Console.WriteLine(message); // Вывод ошибки в консоль
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
