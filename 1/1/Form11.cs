using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace _1
{
    public partial class Form11 : Form
    {
        private string connectionString = "Server=ADCLG1;Database=Практика Чекалина;Integrated Security=True;";
        private Form12 addCityForm = null;

        public Form11()
        {
            InitializeComponent();
            LoadCities();
            btnAdd.Click += btnAdd_Click;
            txtCityName.TextChanged += txtCityName_TextChanged;
            FormClosing += Form11_FormClosing;
        }

        private void Form11_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        // Метод для загрузки данных из таблицы Cities
        public void LoadCities()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT CityId AS 'Идентификатор', cityName AS 'Название города' FROM Cities";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridViewCities.DataSource = dataTable; // Привязываем данные к DataGridView
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Обработчик кнопки "Добавить"
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (addCityForm == null || addCityForm.IsDisposed)
            {
                addCityForm = new Form12(this);
                addCityForm.FormClosed += AddCityForm_FormClosed;
                addCityForm.Show();
            }
            else
            {
                addCityForm.BringToFront();
            }
        }

        private void AddCityForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            LoadCities();
            addCityForm = null;
        }

        // Обработчик кнопки "Удалить"
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Проверяем, выбрана ли строка
            if (dataGridViewCities.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите город для удаления!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Прерываем выполнение, если строка не выбрана
            }

            int cityId = Convert.ToInt32(dataGridViewCities.SelectedRows[0].Cells["Идентификатор"].Value);

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Проверяем, есть ли связанные записи в таблице Flights
                    string checkFlightsQuery = @"
                        SELECT COUNT(*) 
                        FROM Flights 
                        WHERE DepartureCityId = @CityId OR ArrivalCityId = @CityId";
                    using (SqlCommand checkFlightsCommand = new SqlCommand(checkFlightsQuery, connection))
                    {
                        checkFlightsCommand.Parameters.AddWithValue("@CityId", cityId);
                        int relatedFlightsCount = Convert.ToInt32(checkFlightsCommand.ExecuteScalar());

                        if (relatedFlightsCount > 0)
                        {
                            MessageBox.Show("Невозможно удалить город, так как он используется в таблице Flights!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return; // Прерываем выполнение, если город используется
                        }
                    }

                    // Удаляем запись из таблицы Cities
                    string deleteCityQuery = "DELETE FROM Cities WHERE CityId = @CityId";
                    using (SqlCommand deleteCityCommand = new SqlCommand(deleteCityQuery, connection))
                    {
                        deleteCityCommand.Parameters.AddWithValue("@CityId", cityId);
                        deleteCityCommand.ExecuteNonQuery();
                    }

                    MessageBox.Show("Город успешно удален!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCities(); // Обновляем DataGridView

                    // Переводим фокус на текстовое поле txtCityName
                    txtCityName.Focus();
                }
                catch (Exception ex)
                {
                    // Обрабатываем ошибку только в блоке catch
                    MessageBox.Show("Ошибка при удалении города: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Обработчик события TextChanged для поля ввода города
        private void txtCityName_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtCityName.Text;

            // Фильтруем данные в DataGridView на основе введенного текста
            FilterDataGridView(searchText, "Название города"); // Указываем имя столбца
        }

        // Метод для фильтрации данных в DataGridView
        private void FilterDataGridView(string searchText, string columnName)
        {
            if (dataGridViewCities.DataSource is DataTable dataTable)
            {
                DataView dv = dataTable.DefaultView;
                // Устанавливаем фильтр, заключая имя столбца в квадратные скобки, если оно содержит пробелы
                string filterColumnName = columnName.Contains(" ") ? $"[{columnName}]" : columnName;
                dv.RowFilter = string.IsNullOrEmpty(searchText)
                    ? string.Empty
                    : $"{filterColumnName} LIKE '%{searchText}%'";
            }
        }
    }
}