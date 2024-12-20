using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace _1
{
    public partial class Form8 : Form
    {
        private string connectionString = "Server=ADCLG1;Database=Практика Чекалина;Integrated Security=True;";

        public Form8()
        {
            InitializeComponent();
            LoadUsers();
            txtSearchLastName.TextChanged += txtSearchLastName_TextChanged;
            btnEditData.Click += btnEditData_Click;
            FormClosing += Form8_FormClosing; 
        }
        private void Form8_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        // Метод для загрузки данных в DataGridView
        private void LoadUsers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"
                SELECT UserId, Email, Password, FirstName, LastName, RoleId 
                FROM Users";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Привязка данных к DataGridView
                        dataGridViewUsers.DataSource = dataTable;

                        // Скрываем столбец с паролем
                        if (dataGridViewUsers.Columns["Password"] != null)
                        {
                            dataGridViewUsers.Columns["Password"].Visible = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Обработчик события изменения текста 
        private void txtSearchLastName_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearchLastName.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                LoadUsers();
            }
            else
            {
                SearchUsersByLastName(searchText);
            }
        }

        // Метод для поиска пользователей по фамилии
        private void SearchUsersByLastName(string lastName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"
                        SELECT UserId, Email, Password, FirstName, LastName, RoleId 
                        FROM Users 
                        WHERE LastName LIKE @lastName + '%'";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@lastName", lastName);

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Привязка данных к DataGridView
                        dataGridViewUsers.DataSource = dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при поиске данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Обработчик события нажатия кнопки "Изменить данные"
        private void btnEditData_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count > 0)
            {
                int userId = Convert.ToInt32(dataGridViewUsers.SelectedRows[0].Cells["UserId"].Value);
                Form9 editForm = new Form9(userId);
                this.Hide();
                editForm.ShowDialog();
                LoadUsers();
                this.Show();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для редактирования.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}