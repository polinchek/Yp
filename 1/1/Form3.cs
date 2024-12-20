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
    public partial class Form3 : Form
    {
        
        public Form3()
        {
            InitializeComponent();
           
       
            btnSearchTickets.Click += btnSearchTickets_Click;
            btnMyBookings.Click += btnMyBookings_Click;
            FormClosing += Form3_FormClosing;
        }
       

        //"Поиск авиабилетов"
        private void btnSearchTickets_Click(object sender, EventArgs e)
        {
            
            Form4 searchTicketsForm = new Form4();
            searchTicketsForm.Show();
            this.Hide();
        }

        //"Мои бронирования"
        private void btnMyBookings_Click(object sender, EventArgs e)
        {
            
            Form5 myBookingsForm = new Form5();
            myBookingsForm.Show();
            this.Hide();
        }
        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            Application.Exit();
        }
      
    }
}