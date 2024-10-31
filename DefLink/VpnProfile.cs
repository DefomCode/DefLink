using System;

namespace DefLink
{
    public class VpnProfile
    {
        public string UUID { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string Security { get; set; }
        public string Type { get; set; }

        public static VpnProfile Parse(string uri)
        {
            // Убираем префикс
            var uriWithoutPrefix = uri.Substring("vless://".Length);

            // Декодируем base64
            var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(uriWithoutPrefix));

            // Здесь нужно реализовать логику парсинга
            // Например, использовать строки для разделения на части

            // Примерная логика парсинга
            var parts = decoded.Split('@');
            var credentials = parts[0].Split(':');
            var addressInfo = parts[1].Split(':');

            return new VpnProfile
            {
                UUID = credentials[0],
                Address = addressInfo[0],
                Port = int.Parse(addressInfo[1].Split('?')[0]),
                // Заполните другие параметры по необходимости
            };
        }
    }
}
