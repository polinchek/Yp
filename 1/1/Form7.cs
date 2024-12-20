using System;
using System.Windows.Forms;

namespace _1
{
    public partial class Form7 : Form
    {
        private Form8 manageUsersFormInstance = null;

        public Form7()
        {
            InitializeComponent();
            btnManageUsers.Click += btnManageUsers_Click; 
            FormClosing += Form7_FormClosing; 
        }
        private void btnSearchTickets_Click(object sender, EventArgs e)
        {
            Form4 searchTicketsForm = new Form4();
            searchTicketsForm.Show();
            this.Hide();
        }

        // Обработчик кнопки "Мои бронирования"
        private void btnMyBookings_Click(object sender, EventArgs e)
        {
            Form5 myBookingsForm = new Form5();
            myBookingsForm.Show();
            this.Hide();
        }

        private void Form7_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        // Обработчик нажатия кнопки "Управление пользователями"
        private void btnManageUsers_Click(object sender, EventArgs e)
        {         
            if (manageUsersFormInstance == null || manageUsersFormInstance.IsDisposed)
            {
                manageUsersFormInstance = new Form8();
                manageUsersFormInstance.FormClosed += ManageUsersForm_FormClosed;
                manageUsersFormInstance.Show();
            }
            else
            {
                manageUsersFormInstance.BringToFront();
            }
            this.Hide();
        }
        private void ManageUsersForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Show();
        }
    }
}