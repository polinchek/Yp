    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;

    namespace _1
    {
        public partial class Form4 : Form
        {
        private string connectionString = "Server=ADCLG1;Database=Практика Чекалина;Integrated Security=True;";

        public Form4()
            {
                InitializeComponent();
                LoadFlights();
            FormClosing += Form4_FormClosing;
        }
       
        private void btnMain_Click(object sender, EventArgs e)
        {
             {
                if (UserSession.RoleId == 2)
                {
                   
                    Form7 adminForm = new Form7();
                    adminForm.Show();
                    this.Hide();
                }
                else if (UserSession.RoleId == 3) 
                {
                    Form10 citiesForm = new Form10();
                    citiesForm.Show();
                    this.Hide();
                }
                else
                {                   
                    Form3 mainForm = new Form3();
                    mainForm.Show();
                    this.Hide();
                }
            }
        }
        private void Form4_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        // Метод для загрузки данных из таблицы Flights
        private void LoadFlights()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT flightId, departureCityId, arrivalCityId, departureCity, arrivalCity, date, departureTime, price, duration FROM Flights";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Настройка заголовков столбцов
                dt.Columns["flightId"].ColumnName = "flightId";
                dt.Columns["departureCityId"].ColumnName = "Идентификатор города отправления";
                dt.Columns["arrivalCityId"].ColumnName = "Идентификатор города прибытия";
                dt.Columns["departureCity"].ColumnName = "Город отправления";
                dt.Columns["arrivalCity"].ColumnName = "Город прибытия";
                dt.Columns["date"].ColumnName = "Дата";
                dt.Columns["departureTime"].ColumnName = "Время отправления";
                dt.Columns["price"].ColumnName = "Цена";
                dt.Columns["duration"].ColumnName = "Продолжительность полета";

                dgvFlights.DataSource = dt;

                // Скрытие столбцов
                dgvFlights.Columns["Идентификатор города отправления"].Visible = false;
                dgvFlights.Columns["Идентификатор города прибытия"].Visible = false;
            }
        }
        private void btnFilter_Click(object sender, EventArgs e)
        {
            FilterFlights();
        }

        // Метод для фильтрации данных
        private void FilterFlights()
        {
            string arrivalCity = txtCity.Text.Trim(); 
            DateTime date = dtpDate.Value.Date; 

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT flightId, departureCityId, arrivalCityId,departureCity, arrivalCity, date, departureTime, price, duration " +
                               "FROM Flights WHERE arrivalCity = @arrivalCity AND date = @date";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@arrivalCity", arrivalCity);
                adapter.SelectCommand.Parameters.AddWithValue("@date", date);

                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dt.Columns["flightId"].ColumnName = "flightId";
                dt.Columns["departureCityId"].ColumnName = "Идентификатор города отправления";
                dt.Columns["arrivalCityId"].ColumnName = "Идентификатор города прибытия";
                dt.Columns["departureCity"].ColumnName = "Город отправления";
                dt.Columns["arrivalCity"].ColumnName = "Город прибытия";
                dt.Columns["date"].ColumnName = "Дата";
                dt.Columns["departureTime"].ColumnName = "Время отправления";
                dt.Columns["price"].ColumnName = "Цена";
                dt.Columns["duration"].ColumnName = "Продолжительность полета";

                dgvFlights.DataSource = dt;

                // Скрытие столбцов
               
                dgvFlights.Columns["Идентификатор города отправления"].Visible = false;
                dgvFlights.Columns["Идентификатор города прибытия"].Visible = false;
                dgvFlights.DataSource = dt;
            }
        }

      
       

        // Обработчик нажатия кнопки "Забронировать"
        private void btnBook_Click(object sender, EventArgs e)
        {
            // Проверка, выбран ли рейс
            if (dgvFlights.SelectedRows.Count > 0)
            {
                // Получаем flightId выбранного рейса
                int flightId = Convert.ToInt32(dgvFlights.SelectedRows[0].Cells["flightId"].Value);
                Form6 bookingForm = new Form6(flightId);
                bookingForm.Show();
                this.Hide(); 
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите рейс для бронирования.");
            }

        }
        //Метод фильтрации
        private void FilterDataGridView(string searchText, string columnName)
        {
            if (dgvFlights.DataSource is DataTable dataTable)
            {
                // Создаем DataView из DataTable
                DataView dv = dataTable.DefaultView;

                // Устанавливаем фильтр, заключая имя столбца в квадратные скобки, если оно содержит пробелы
                string filterColumnName = columnName.Contains(" ") ? $"[{columnName}]" : columnName;//содержит ли имя столбца пробелы
                dv.RowFilter = string.IsNullOrEmpty(searchText)
                    ? string.Empty
                    : $"{filterColumnName} LIKE '%{searchText}%'";
            }
        }
        
        private void txtCity_TextChanged(object sender, EventArgs e)
        {

            string searchText = txtCity.Text;

            // Фильтруем данные в DataGridView на основе введенного текста
            FilterDataGridView(searchText, "Город прибытия"); 
        }
    }
        }
    