using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace _1
{
    public partial class Form12 : Form
    {
        private string connectionString = "Server=ADCLG1;Database=Практика Чекалина;Integrated Security=True;";
        private Form11 parentForm;
        public Form12(Form11 parentForm)
        {
            InitializeComponent();
            this.parentForm = parentForm; 
            btnSave.Click += btnSave_Click;
        }

        // Обработчик кнопки "Сохранить"
        private void btnSave_Click(object sender, EventArgs e)
        {
            string cityName = txtCityName.Text;

            // Проверка, что поле заполнено
            if (string.IsNullOrWhiteSpace(cityName))
            {
                MessageBox.Show("Пожалуйста, введите название города!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Добавляем город в базу данных
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO Cities (cityName) VALUES (@cityName)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@cityName", cityName);
                    command.ExecuteNonQuery();

                    MessageBox.Show("Город успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    parentForm.Show(); 
                    parentForm.LoadCities(); 
                    this.Close(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении города: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}