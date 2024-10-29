using System;
using System.Data.SqlClient;
using System.Windows;

namespace DefLink
{
    public class DatabaseManager
    {
        private string connectionString;

        public DatabaseManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool CheckLogin(string login, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Account WHERE Login = @login AND Password = @password";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@password", password); // Учтите, что хранить пароли в открытом виде не рекомендуется

                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public bool IsUserLoggedIn()
        {
            // Проверка наличия сохраненного ID пользователя
            return Application.Current.Properties.Contains("UserId");
        }

        // Метод для регистрации нового пользователя
        public bool RegisterUser(string login, string email, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Account (Login, Mail, Password) VALUES (@login, @mail, @password)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@mail", email);
                    command.Parameters.AddWithValue("@password", password); // Снова, хранить пароли в открытом виде небезопасно

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // Возвращает true, если регистрация прошла успешно
                }
            }
        }
    }
}
