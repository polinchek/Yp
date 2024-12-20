using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace _1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            btnRegister.Click += btnRegister_Click;
            textBox5.Mask = "+7 (000) 000-00-00";
            txtEmail.Validating += txtEmail_Validating;
            FormClosing += Form2_FormClosing;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        public bool RegisterUser(string firstName, string lastName, string phone, string email, string password, string confirmPassword)
        {
            // Проверка на пустые поля
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                throw new ArgumentException("Все поля обязательны для заполнения.");
            }

            // Проверка email
            if (!email.Contains("@"))
            {
                throw new ArgumentException("Некорректный email.");
            }

            // Проверка совпадения пароля и подтверждения пароля
            if (password != confirmPassword)
            {
                throw new ArgumentException("Пароль и подтверждение пароля не совпадают.");
            }

            // Успешная регистрация
            return true;
        }
        private void txtEmail_Validating(object sender, CancelEventArgs e)
        {
            // Проверка, что email введён корректно
            if (!ValidEmailAddress(txtEmail.Text))
            {
                e.Cancel = true;
                txtEmail.Select(0, txtEmail.Text.Length);
                MessageBox.Show("Некорректный email. Пожалуйста, введите корректный email.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Проверка, что строка является валидным адресом email
        private bool ValidEmailAddress(string email)
        {
            string regexPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            return Regex.IsMatch(email, regexPattern);
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Получение данных из полей формы
            string email = txtEmail.Text;
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;
            string firstName = txtFirstName.Text;
            string lastName = txtLastName.Text;
            string phone = textBox5.Text;

            // Проверка, что все поля заполнены
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword) || string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Проверка совпадения паролей
            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Хэширование пароля с использованием SHA-512
            string hashedPassword = HashPassword(password);

            // Добавление пользователя в базу данных
            if (RegisterUser(email, hashedPassword, firstName, lastName))
            {
                // После успешной регистрации переход на форму 1
                Form1 mainForm = new Form1();
                mainForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Ошибка регистрации. Попробуйте снова.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                Debug.WriteLine(hash);
                return hash;
            }
        }

        // Метод для регистрации пользователя
        private bool RegisterUser(string email, string hashedPassword, string firstName, string lastName)
        {
            string connectionString = "Server=ADCLG1;Database=Практика Чекалина;Integrated Security=True;";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL-запрос для добавления пользователя в таблицу Users
                    string query = "INSERT INTO Users (Email, Password, FirstName, LastName) VALUES (@email, @password, @firstName, @lastName)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        // Параметры для запроса
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@password", hashedPassword); // Хэшированный пароль
                        command.Parameters.AddWithValue("@firstName", firstName);
                        command.Parameters.AddWithValue("@lastName", lastName);

                        // Выполнение запроса
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0; // Возвращаем true, если запись добавлена успешно
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}