
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Food_Stuffs
{
    public partial class User_Registration : Form
    {
        public User_Registration()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
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
                else if (control is DateTimePicker)
                {
                    if (((DateTimePicker)control).Value == null)
                    {
                        isAnyEmpty = true;
                        break;
                    }
                }
                else if (control is ComboBox)
                {
                    if (((ComboBox)control).SelectedIndex == -1)
                    {
                        isAnyEmpty = true;
                        break;
                    }
                }
            }
            if (isAnyEmpty)
            {
                MessageBox.Show("One or more fields are empty, Please fill it before submitting");
            }
            else
            {
                SqlConnection con = new SqlConnection("Server=localhost;Database=food;User ID=root;Password=;");
                con.Open();
                string insertQuery = "INSERT INTO users (name, username, password, email, phonenumber, occupation, gender, dateofbirth, photo) VALUES (@name, @username, @password, @email, @phonenumber, @occupation, @gender, @dateofbirth, @photo)";
                SqlCommand cmd = new SqlCommand(insertQuery, con);
                cmd.Parameters.AddWithValue("@name", textBox1.Text);
                cmd.Parameters.AddWithValue("@username", textBox2.Text);
                cmd.Parameters.AddWithValue("@password", textBox3.Text);
                cmd.Parameters.AddWithValue("@email", textBox5.Text);
                cmd.Parameters.AddWithValue("@phonenumber", textBox6.Text);
                cmd.Parameters.AddWithValue("@occupation", textBox7.Text);
                cmd.Parameters.AddWithValue("@gender", comboBox1.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@dateofbirth", dateTimePicker1.Value);
                // Assuming you have a method to convert Image to byte[] for storage in the database
                cmd.Parameters.AddWithValue("@photo", ConvertImageToBytes(pictureBox1.Image));
                cmd.ExecuteNonQuery();
                MessageBox.Show("User Registered Successfully", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Method to convert Image to byte[]
        private byte[] ConvertImageToBytes(Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(openFileDialog.FileName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}