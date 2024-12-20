using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1
{
    public partial class Form10 : Form
    {
        public Form10()
        {
            InitializeComponent();
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
        private void btnCitys_Click(object sender, EventArgs e)
        {
            Form11 searchTicketsForm = new Form11();
            searchTicketsForm.Show();
            this.Hide();
        }
        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
