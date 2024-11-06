using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

public class ConfigGenerator
{
    public static void UpdateConfig(string uuid, string serverAddress, string publicKey, string label)
    {
        string configPath = @"xray\config.json";

        // Чтение конфигурации
        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath);
            var config = JsonConvert.DeserializeObject<Config>(json);

            // Обновление значений внутри settings.settings
            var inbound = config.Inbounds.FirstOrDefault();
            if (inbound != null)
            {
                // В settings.settings внутри inbound
                var settings = inbound.Settings;
                if (settings != null)
                {
                    // Обновляем UUID, ServerAddress, PublicKey, Label
                    settings.Id = uuid;  // UUID
                    settings.ServerAddress = serverAddress;  // ServerAddress
                    settings.PublicKey = publicKey;  // PublicKey
                    settings.Label = label;  // Label
                }
            }

            // Сохранение изменений обратно в файл
            string updatedJson = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(configPath, updatedJson);

            Console.WriteLine("Конфиг обновлён!");
        }
        else
        {
            Console.WriteLine("Конфигурационный файл не найден.");
        }
    }
}
