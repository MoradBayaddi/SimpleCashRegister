using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CashRegister1.MainClass;

namespace CashRegister1
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter a username and password !");
                return;
            }
            else
            {
                LoginService loginService = new LoginService("cashiers.json");

                // Perform the login
                if (loginService.Login(username, password))
                {
                    MessageBox.Show("Login successful");               
                    this.Hide();

                    Form1 form1 = new Form1();
                    form1.Show();
                }
                else
                {
                    MessageBox.Show("Invalid username or password");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter a username and password !");
                return;
            }
            else
            {
                MainClass.LoginService loginService = new MainClass.LoginService("cashiers.json");

                if (loginService.Register(username, password) == true)
                {
                    MessageBox.Show("Registration successful: " + username + " .");
                }
                else
                {
                    MessageBox.Show("user already taken");
                }
            }
        }

    }
}
