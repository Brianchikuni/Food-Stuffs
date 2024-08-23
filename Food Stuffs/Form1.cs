using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Food_Stuffs
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialization code can be placed here if needed.
        }

        private void addStudentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the Capture class is properly initialized
                Capture capture = new Capture();
                capture.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while trying to add a student: " + ex.Message);
            }
        }

        private void serverFoodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the Verify class is properly initialized
                Verify verify = new Verify();
                verify.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while trying to verify food: " + ex.Message);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Cleanly close the application
            Application.Exit();
        }
    }
}
