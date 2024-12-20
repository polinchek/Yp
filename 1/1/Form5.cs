using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;

namespace _1
{
    public partial class Form5 : Form
    {
        private string connectionString = "Server=ADCLG1;Database=Практика Чекалина;Integrated Security=True;";

        public Form5()
        {
            InitializeComponent();
            LoadBookings(); 
            FormClosing += Form5_FormClosing; 
            btnShowDetails.Click += btnShowDetails_Click; 
        }

        private void Form5_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        // Метод для загрузки истории бронирований
        private void LoadBookings()
        {
            // Используем userId из UserSession
            int userId = UserSession.UserId;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                    p.passengerId, 
                    p.bookingId, 
                    p.arrivalCity, 
                    p.firstName, 
                    p.lastName, 
                    p.dateOfBirth, 
                    p.passportNumber,
					b.userId
                FROM 
                    Passengers p
                LEFT 
				JOIN 
                    Bookings b ON p.bookingId = b.bookingId
				where b.userId = @userId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    Debug.WriteLine(userId);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Привязываем данные к DataGridView
                    dgvBookings.DataSource = dt;

                    // Проверяем, есть ли данные
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Нет данных для отображения.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
                }
            }
        }

        //"Отмена бронирования"
        private void btnCancelBooking_Click(object sender, EventArgs e)
        {
            if (dgvBookings.SelectedRows.Count > 0)
            {
                // Получаем bookingId выбранной строки
                int bookingId = Convert.ToInt32(dgvBookings.SelectedRows[0].Cells["bookingId"].Value);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Удаляем данные из таблицы Passengers
                    string deletePassengersQuery = "DELETE FROM Passengers WHERE bookingId = @bookingId";
                    SqlCommand deletePassengersCmd = new SqlCommand(deletePassengersQuery, conn);
                    deletePassengersCmd.Parameters.AddWithValue("@bookingId", bookingId);
                    deletePassengersCmd.ExecuteNonQuery();

                    // Удаляем данные из таблицы Bookings
                    string deleteBookingsQuery = "DELETE FROM Bookings WHERE bookingId = @bookingId";
                    SqlCommand deleteBookingsCmd = new SqlCommand(deleteBookingsQuery, conn);
                    deleteBookingsCmd.Parameters.AddWithValue("@bookingId", bookingId);
                    deleteBookingsCmd.ExecuteNonQuery();

                    MessageBox.Show("Данные успешно удалены!");
                    LoadBookings(); 
                }
            }
            else
            {
                MessageBox.Show("Выберите строку для удаления.");
            }
        }

        // Обработчик нажатия кнопки "Вернуться в главное меню"
        private void btnBackToMainMenu_Click(object sender, EventArgs e)
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

        // Обработчик события выбора строки в DataGridView
        private void dgvBookings_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvBookings.Rows[e.RowIndex];

                // Получаем данные из выбранной строки
                int passengerId = Convert.ToInt32(row.Cells["passengerId"].Value);
                int bookingId = Convert.ToInt32(row.Cells["bookingId"].Value);
                string firstName = row.Cells["firstName"].Value.ToString();
                string lastName = row.Cells["lastName"].Value.ToString();
                string passportNumber = row.Cells["passportNumber"].Value.ToString();
                string arrivalCity = row.Cells["arrivalCity"].Value.ToString();
                DateTime dateOfBirth = Convert.ToDateTime(row.Cells["dateOfBirth"].Value);

                // Отображаем детали бронирования в MessageBox
                string details = $"Номер бронирования: {bookingId}\n" +
                                 $"Пассажир: {firstName} {lastName}, Паспорт: {passportNumber}\n" +
                                 $"Дата рождения: {dateOfBirth.ToShortDateString()}\n" +
                                 $"Город прибытия: {arrivalCity}";

                MessageBox.Show(details, "Детали бронирования");
            }
        }


        private void btnShowDetails_Click(object sender, EventArgs e)
        {
            if (dgvBookings.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvBookings.SelectedRows[0];

                // Получаем данные из выбранной строки
                int passengerId = Convert.ToInt32(row.Cells["passengerId"].Value);
                int bookingId = Convert.ToInt32(row.Cells["bookingId"].Value);
                string firstName = row.Cells["firstName"].Value.ToString();
                string lastName = row.Cells["lastName"].Value.ToString();
                string passportNumber = row.Cells["passportNumber"].Value.ToString();
                string arrivalCity = row.Cells["arrivalCity"].Value.ToString();
                DateTime dateOfBirth = Convert.ToDateTime(row.Cells["dateOfBirth"].Value);

                // Отображаем детали бронирования в MessageBox
                string details = $"Номер бронирования: {bookingId}\n" +
                                 $"Пассажир: {firstName} {lastName}, Паспорт: {passportNumber}\n" +
                                 $"Дата рождения: {dateOfBirth.ToShortDateString()}\n" +
                                 $"Город прибытия: {arrivalCity}";

                MessageBox.Show(details, "Детали бронирования");
            }
            else
            {
                MessageBox.Show("Выберите строку для просмотра деталей.");
            }
        }
    }
}