using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace _1
{
    public partial class Form9 : Form
    {
        private int userId;
        private string connectionString = "Server=ADCLG1;Database=Практика Чекалина;Integrated Security=True;";

        public Form9(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            LoadUserData();
            LoadRoles(); // Загружаем роли в ComboBox
            btnSave.Click += btnSave_Click;
            FormClosing += Form9_FormClosing;
            txtEmail.Validating += txtEmail_Validating;
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

        // Проверка, что строка является адресом
        private bool ValidEmailAddress(string email)
        {
            string regexPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            return Regex.IsMatch(email, regexPattern);
        }

        private void Form9_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form7 form7 = Application.OpenForms["Form7"] as Form7;
            if (form7 != null)
            {
                form7.Show();
            }
        }

        // Метод для загрузки данных пользователя
        private void LoadUserData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"
                        SELECT Email, FirstName, LastName, RoleId 
                        FROM Users 
                        WHERE UserId = @userId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtEmail.Text = reader["Email"].ToString();
                                txtFirstName.Text = reader["FirstName"].ToString();
                                txtLastName.Text = reader["LastName"].ToString();

                                // Устанавливаем выбранную роль в ComboBox
                                int roleId = Convert.ToInt32(reader["RoleId"]);
                                cmbRole.SelectedValue = roleId;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Метод для загрузки ролей в ComboBox
        private void LoadRoles()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"
                        SELECT role_id, role_name 
                        FROM Roles";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable rolesTable = new DataTable();
                        adapter.Fill(rolesTable);

                        // Привязываем данные к ComboBox
                        cmbRole.DisplayMember = "role_name"; // Отображаемое значение
                        cmbRole.ValueMember = "role_id";     // Значение, которое будет использоваться в коде
                        cmbRole.DataSource = rolesTable;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке ролей: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Обработчик события нажатия кнопки "Сохранить"
        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"
                        UPDATE Users 
                        SET Email = @email, FirstName = @firstName, LastName = @lastName, RoleId = @roleId 
                        WHERE UserId = @userId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@email", txtEmail.Text);
                        command.Parameters.AddWithValue("@firstName", txtFirstName.Text);
                        command.Parameters.AddWithValue("@lastName", txtLastName.Text);
                        command.Parameters.AddWithValue("@roleId", cmbRole.SelectedValue); // Используем выбранное значение из ComboBox
                        command.Parameters.AddWithValue("@userId", userId);

                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Данные успешно обновлены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}