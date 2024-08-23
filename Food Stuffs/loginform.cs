using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace Food_Stuffs
{
    public partial class loginform : Form
    {
        public loginform()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool isAnyEmpty = false;
            foreach (Control control in this.Controls)
            {
                if (control is TextBox)
                {
                    if (string.IsNullOrEmpty(control.Text))
                    {
                        isAnyEmpty = true;
                        break;
                    }
                }
            }
            if (isAnyEmpty)
            {
                MessageBox.Show("one or more fields  are empty, Please fill it before submitting");
            }
            else
            {

                SqlConnection con = new SqlConnection("Server=localhost;Database=food;User ID=root;Password=;Port=3306;");
                string querry = "SELECT * from Users where username = '" + textBox1.Text.Trim() + "'and password = '" + textBox2.Text.Trim();
                SqlDataAdapter sda =new SqlDataAdapter(querry, con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    // Create an instance of the new form
                    Form1 form1 = new Form1();

                    // Show the new form
                    form1.Show();

                    // Close the current form
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Wrong Password"); 
                        
                }

                
            }
            

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            User_Registration userRegistration = new User_Registration();
            userRegistration.ShowDialog();


        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = string.Empty;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
