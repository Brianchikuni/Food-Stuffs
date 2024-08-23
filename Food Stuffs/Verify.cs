using System;
using System.Collections.Generic;
using System.Data.SqlClient; // Assuming SQL Server; adjust for other databases
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DPFP;
using DPFP.Capture;
using DPFP.Processing;
using DPFP.Verification;
using MySql.Data.MySqlClient;

namespace Food_Stuffs
{
    public partial class Verify : Form, DPFP.Capture.EventHandler
    {
        private DPFP.Capture.Capture Capturer;
        private DPFP.Verification.Verification Verificator;
        private List<DPFP.Template> Templates;
        private List<string> TemplateFileNames;
        private const string EnrollmentFolder = "enrollments";
        private const string ConnectionString = "Server=localhost;Database=food1;User ID=root;Password=;Port=3306;"; // Updated connection string

        private void DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            // Assuming you have a DateTimePicker control named 'dateTimePicker1'
            dateTimePicker1.Value = DateTime.Now;
        }
        public Verify()
        {
            InitializeComponent();
            Init();
            StartCapture();
        }

        private void Verify_Load(object sender, EventArgs e)
        {
        }

        private void Init()
        {
            try
            {
                Capturer = new DPFP.Capture.Capture();
                Capturer.EventHandler = this;
                Verificator = new DPFP.Verification.Verification();
                Templates = new List<DPFP.Template>();
                TemplateFileNames = new List<string>();
                LoadTemplates();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadTemplates()
        {
            try
            {
                if (Directory.Exists(EnrollmentFolder))
                {
                    foreach (var filePath in Directory.GetFiles(EnrollmentFolder, "*.dat"))
                    {
                        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            DPFP.Template template = new DPFP.Template();
                            template.DeSerialize(fs);
                            Templates.Add(template);
                            TemplateFileNames.Add(Path.GetFileNameWithoutExtension(filePath));
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No templates found. Please enroll fingerprints first.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load templates. Error: " + ex.Message);
            }
        }

        protected void StartCapture()
        {
            if (Capturer != null)
            {
                try
                {
                    Capturer.StartCapture();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Cannot start capture. Error: " + ex.Message);
                }
            }
        }

        protected void StopCapture()
        {
            if (Capturer != null)
            {
                try
                {
                    Capturer.StopCapture();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Cannot stop capture. Error: " + ex.Message);
                }
            }
        }

        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            Process(Sample);
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber) { }
        public void OnFingerTouch(object Capture, string ReaderSerialNumber) { }
        public void OnReaderConnect(object Capture, string ReaderSerialNumber) { }
        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber) { }
        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback) { }

        protected void Process(DPFP.Sample Sample)
        {
            DrawPicture(ConvertSampleToBitmap(Sample));
            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Verification);

            if (features != null && Templates.Count > 0)
            {
                bool verified = false;
                int index = -1;
                for (int i = 0; i < Templates.Count; i++)
                {
                    DPFP.Verification.Verification.Result result = new DPFP.Verification.Verification.Result();
                    Verificator.Verify(features, Templates[i], ref result);

                    if (result.Verified)
                    {
                        verified = true;
                        index = i;
                        break;
                    }
                }

                if (verified && index >= 0)
                {
                    string templateFileName = TemplateFileNames[index];
                    DisplayStudentData(templateFileName);
                }
                else
                {
                    ClearData();
                    MessageBox.Show("Fingerprint verification failed.");
                }
            }
            else
            {
                MessageBox.Show("No templates to verify against.");
            }
        }

        protected DPFP.FeatureSet ExtractFeatures(DPFP.Sample Sample, DPFP.Processing.DataPurpose Purpose)
        {
            DPFP.Processing.FeatureExtraction extractor = new DPFP.Processing.FeatureExtraction();
            DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
            DPFP.FeatureSet features = new DPFP.FeatureSet();
            extractor.CreateFeatureSet(Sample, Purpose, ref feedback, ref features);
            if (feedback == DPFP.Capture.CaptureFeedback.Good)
                return features;
            else
                return null;
        }

        protected Bitmap ConvertSampleToBitmap(DPFP.Sample Sample)
        {
            DPFP.Capture.SampleConversion convertor = new DPFP.Capture.SampleConversion();
            Bitmap bitmap = null;
            convertor.ConvertToPicture(Sample, ref bitmap);
            return bitmap;
        }

        private void DrawPicture(Bitmap bitmap)
        {
            this.Invoke(new Action(delegate ()
            {
                Picture.Image = new Bitmap(bitmap, Picture.Size);
            }));
        }

        private void DisplayStudentData(string templateFileName)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT FullName, StudentClass FROM Students WHERE template_filename = @template_filename";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@template_filename", templateFileName);
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                RenderData(reader);

                            }
                            else
                            {
                                MessageBox.Show("No student data found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving student data: " + ex.Message);
            }
        }

        private void RenderData(MySqlDataReader reader)
        {
            this.Invoke(new Function(delegate ()
            {

                string fullName = reader["fullname"].ToString();
                string studentClass = reader["studentclass"].ToString();
                lblFullName.Text = "Full Name: " + fullName;
                lblStudentClass.Text = "Class: " + studentClass;

            }));

        }

        private void ClearData()
        {
            this.Invoke(new Function(delegate ()
            {
                lblFullName.Text = "Full Name: Not found";
                lblStudentClass.Text = "Class: Not found";

            }));
        }

        private delegate void Function();

        private void Verify_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopCapture();
        }
    }
}
