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
                string query = "SELECT COUNT(*) FROM [User] WHERE Login = @login AND Password = @password";

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
                string query = "INSERT INTO [User] (Login, Email, Password) VALUES (@login, @email, @password)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@password", password); // Помните о безопасности хранения паролей

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // Возвращает true, если регистрация прошла успешно
                }
            }
        }

        public int GetUserIdByLogin(string login) // Код который берет айди пользователя если он существует
        {
            int userId = 0; // Стандартное значение если пользователь не вошёл
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT ID_User FROM [User] WHERE Login = @login"; // поиск айди по логину

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", login); // присвоить логин из базы данных 
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        userId = Convert.ToInt32(result); // Явно преобразует айди пользователя в Инт 
                    }
                }
            }
            return userId; // возвращаем айди пользователя
        }


    }
}
