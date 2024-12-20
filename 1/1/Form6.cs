using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;

namespace _1
{
    public partial class Form6 : Form
    {
        private int flightID;
        private string arrivalCity;
        private string connectionString = "Server=ADCLG1;Database=Практика Чекалина;Integrated Security=True;";

        public Form6(int flightID)
        {
            InitializeComponent();
            this.flightID = flightID;
            this.arrivalCity = GetArrivalCity(flightID);
            FormClosing += Form6_FormClosing;
        }

        private bool IsUserExists(int userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE userId = @userId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        private void Form6_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        // Метод для получения arrivalCity из базы данных
        private string GetArrivalCity(int flightID)
        {
            string arrivalCity = string.Empty;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT arrivalCity FROM Flights WHERE flightId = @flightId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@flightId", flightID);

                    // Получаем arrivalCity из базы данных
                    arrivalCity = cmd.ExecuteScalar()?.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получении данных о полете: {ex.Message}");
                }
            }

            return arrivalCity;
        }

        // Метод для проверки наличия бронирования с таким же именем и фамилией
        private bool IsBookingExists(string firstName, string lastName, int flightID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Passengers " +
                               "JOIN Bookings ON Passengers.bookingId = Bookings.bookingId " +
                               "WHERE Passengers.firstName = @firstName AND Passengers.lastName = @lastName " +
                               "AND Bookings.flightId = @flightId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@firstName", firstName);
                cmd.Parameters.AddWithValue("@lastName", lastName);
                cmd.Parameters.AddWithValue("@flightId", flightID);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        // Обработчик нажатия кнопки "Забронировать"
        private void btnBook_Click(object sender, EventArgs e)
        {
            // Получаем данные из текстовых полей
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string email = txtEmail.Text.Trim();
            string passport = txtPassport.Text.Trim();
            DateTime birthDate = dtpBirthDate.Value;

            // Проверка введенных данных
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(passport))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            int userId = UserSession.UserId;
            if (userId <= 0)
            {
                MessageBox.Show("Пользователь не авторизован. Пожалуйста, войдите в систему.");
                return;
            }

            if (!IsUserExists(userId))
            {
                MessageBox.Show("Пользователь не найден. Пожалуйста, зарегистрируйтесь.");
                return;
            }

            // Проверка наличия бронирования с таким же именем и фамилией
            if (IsBookingExists(firstName, lastName, flightID))
            {
                MessageBox.Show("Пассажир с таким именем и фамилией уже забронировал билет на этот рейс.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Сохранение данных в таблицы Bookings и Passengers
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 1. Вставка данных в таблицу Bookings
                    string queryBookings = "INSERT INTO Bookings (userId, flightId, bookingDate, status, birthDate) " +
                                           "VALUES (@userId, @flightId, @bookingDate, @status, @birthDate); " +
                                           "SELECT SCOPE_IDENTITY();";

                    SqlCommand cmdBookings = new SqlCommand(queryBookings, conn, transaction);
                    cmdBookings.Parameters.AddWithValue("@userId", userId);
                    cmdBookings.Parameters.AddWithValue("@flightId", flightID);
                    cmdBookings.Parameters.AddWithValue("@bookingDate", DateTime.Now);
                    cmdBookings.Parameters.AddWithValue("@status", "Ожидает");
                    cmdBookings.Parameters.AddWithValue("@birthDate", birthDate);

                    int bookingId = Convert.ToInt32(cmdBookings.ExecuteScalar());

                    // 2. Вставка данных в таблицу Passengers
                    string queryPassengers = "INSERT INTO Passengers (bookingId, arrivalCity, firstName, lastName, dateOfBirth, passportNumber) " +
                                             "VALUES (@bookingId, @arrivalCity, @firstName, @lastName, @dateOfBirth, @passportNumber)";

                    SqlCommand cmdPassengers = new SqlCommand(queryPassengers, conn, transaction);
                    cmdPassengers.Parameters.AddWithValue("@bookingId", bookingId);
                    cmdPassengers.Parameters.AddWithValue("@arrivalCity", arrivalCity);
                    cmdPassengers.Parameters.AddWithValue("@firstName", firstName);
                    cmdPassengers.Parameters.AddWithValue("@lastName", lastName);
                    cmdPassengers.Parameters.AddWithValue("@dateOfBirth", birthDate);
                    cmdPassengers.Parameters.AddWithValue("@passportNumber", passport);

                    cmdPassengers.ExecuteNonQuery();

                    transaction.Commit();

                    MessageBox.Show("Бронирование успешно выполнено!");
                    Form3 mainForm = new Form3();
                    mainForm.Show();
                    this.Hide();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Ошибка при бронировании: {ex.Message}");
                }
            }
        }
    }
}