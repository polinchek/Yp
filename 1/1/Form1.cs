using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace _1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            btnLogin.Click += btnLogin_Click;
            btnRegister.Click += btnRegister_Click;
           }
        // "Вход"
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Text;

            // Проверка, что поля заполнены
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Проверка данных в базе данных
            if (CheckUserCredentials(email, password))
            {
                // Проверяем RoleId пользователя
                if (UserSession.RoleId == 2) 
                {
                    Form7 adminForm = new Form7();
                    adminForm.Show();
                }
                else if (UserSession.RoleId == 3) 
                {
                    Form10 citiesForm = new Form10();
                    citiesForm.Show();
                }
                else
                {                 
                    Form3 mainForm = new Form3();
                    mainForm.Show();
                }

                this.Hide(); 
            }
            else
            {
                // Если данные неверны, вывод ошибки
                MessageBox.Show("Неверный email или пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //"Регистрация"
        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Переход на форму регистрации
            Form2 registrationForm = new Form2();
            registrationForm.Show();
            this.Hide();
        }

        // Метод для проверки данных в базе данных
        private bool CheckUserCredentials(string email, string password)
        {
            // Строка подключения к SQL Server
            string connectionString = "Server=ADCLG1;Database=Практика Чекалина;Integrated Security=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT UserId, RoleId, Password 
                    FROM Users 
                    WHERE Email = @email";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", email);

                    // Получаем данные пользователя
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userId = reader.GetInt32(0); // Получаем UserId
                            int roleId = reader.GetInt32(1); // Получаем RoleId
                            string hashedPasswordFromDB = reader.GetString(2); // Получаем хэшированный пароль из базы данных

                            // Вычисляем хэш введённого пароля
                            string hashedInputPassword = HashPassword(password);

                            // Сравниваем хэши
                            if (hashedPasswordFromDB == hashedInputPassword)
                            {
                                // Сохраняем данные в UserSession
                                UserSession.UserId = userId;
                                UserSession.Email = email;
                                UserSession.RoleId = roleId; // Сохраняем RoleId

                                return true; // Успешная авторизация
                            }
                        }
                    }
                }
            }

            return false; // Неверные данные
        }

        // Метод для хэширования пароля с использованием SHA-512
        private string HashPassword(string password)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                // Преобразуем пароль в массив байтов
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Вычисляем хэш
                byte[] hashBytes = sha512.ComputeHash(passwordBytes);

                // Преобразуем хэш в строку в формате Base64
                string hash = Convert.ToBase64String(hashBytes);

                return hash;
            }
        }
    }
}