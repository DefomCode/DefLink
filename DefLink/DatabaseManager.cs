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
                    command.Parameters.AddWithValue("@password", password);

                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public VpnProfile GetVpnProfile(string profileId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM VpnProfiles WHERE Id = @profileId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@profileId", profileId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new VpnProfile
                            {
                                UUID = reader["UUID"].ToString(),
                                Label = reader["Label"].ToString(),
                                ServerAdress = reader["Address"].ToString(),
                                PublicKey = reader["PublicKey"].ToString()
                            };
                        }
                        else
                        {
                            throw new Exception("Профиль VPN не найден в базе данных.");
                        }
                    }
                }
            }
        }

        public bool IsUserLoggedIn()
        {
            return Application.Current.Properties.Contains("ID_User");
        }

        public bool RegisterUser(string login, string email, string password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Проверяем, существует ли уже такой логин
                    string checkLoginQuery = "SELECT COUNT(*) FROM [User] WHERE Login = @login";
                    using (SqlCommand command = new SqlCommand(checkLoginQuery, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        int loginCount = (int)command.ExecuteScalar();
                        if (loginCount > 0)
                        {
                            throw new Exception("Логин уже существует.");
                        }
                    }

                    // Проверяем, существует ли уже такая почта
                    string checkEmailQuery = "SELECT COUNT(*) FROM [User] WHERE Email = @email";
                    using (SqlCommand command = new SqlCommand(checkEmailQuery, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);
                        int emailCount = (int)command.ExecuteScalar();
                        if (emailCount > 0)
                        {
                            throw new Exception("Почта уже используется.");
                        }
                    }

                    // Если логин и почта уникальны, то выполняем вставку
                    string query = "INSERT INTO [User] (Login, Email, Password) VALUES (@login, @email, @password)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@password", password);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0; // Успешно зарегистрирован
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // Для отладки
                MessageBox.Show(ex.Message); // Показываем пользователю ошибку
                return false; // Возвращаем false при ошибке
            }
        }

        public UserData GetUserDataById(int ID_User)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
        SELECT s.ID_Subscription, v.UUID, v.ServerAddress, v.PublicKey, v.Label, u.Login
        FROM [User] u
        LEFT JOIN Subscription s ON u.ID_User = s.ID_User
        LEFT JOIN VPN v ON s.ID_Subscription = v.ID_Subscription
        WHERE u.ID_User = @ID_User";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_User", ID_User);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserData
                            {
                                ID_User = ID_User,
                                Login = reader["Login"].ToString(),
                                UUID = reader["UUID"]?.ToString() ?? "No UUID",
                                ServerAddress = reader["ServerAddress"]?.ToString() ?? "No Address",
                                PublicKey = reader["PublicKey"]?.ToString() ?? "No PublicKey",
                                Label = reader["Label"]?.ToString() ?? "No Label"
                            };
                        }
                        else
                        {
                            return null; // Если пользователь не найден
                        }
                    }
                }
            }
        }



        public string GetUserNameById(int userId)
        {
            string userName = string.Empty;

            try
            {
                string query = "SELECT Login FROM User WHERE ID_User = @ID_User";  // Запрос к базе данных

                Console.WriteLine("Запрос: " + query);  // Логируем запрос

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID_User", userId);  // Привязка параметра
                        var result = command.ExecuteScalar();  // Выполнение запроса

                        // Логируем результат
                        Console.WriteLine("Результат запроса: " + (result == null ? "null" : result.ToString()));

                        if (result != null)
                        {
                            userName = result.ToString();  // Если результат не null, присваиваем значение
                        }
                        else
                        {
                            Console.WriteLine("Пользователь с ID " + userId + " не найден.");
                            userName = "Пользователь не найден";  // Если результат пустой
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении имени пользователя: " + ex.Message);
                userName = "Ошибка при загрузке данных";
            }

            return userName;
        }





        public int GetUserIdByLogin(string login)
        {
            int ID_User = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT ID_User FROM [User] WHERE Login = @login";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", login);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        ID_User = Convert.ToInt32(result);
                    }
                }
            }
            return ID_User;
        }
    }

    public class UserData
    {
        public int ID_User { get; set; }   // Добавляем ID_User
        public string Login { get; set; }   // Добавляем Login
        public string UUID { get; set; }
        public string ServerAddress { get; set; }
        public string PublicKey { get; set; }
        public string Label { get; set; }
    }

}
