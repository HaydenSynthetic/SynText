using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace TextEdit
{
    public partial class Form1 : Form
    {

        bool mouseDown = false;
        Point lastLocation;
        string filePath;
        bool fileOpened;

        public Form1(string newFile)
        {
            InitializeComponent();

            if (newFile != "") // If app is opened with a text file
            {
                Debug.WriteLine("File opened on startup");

                var fileStream = newFile;

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    textBox.Text = reader.ReadToEnd();
                }
                fileOpened = true;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }





        private void window_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void openFile_Click(object sender, EventArgs e) // On CTRL + O
        {
            Debug.WriteLine("Open file");

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                filePath = openFileDialog.FileName;

                //Read the contents of the file into a stream
                var fileStream = openFileDialog.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    textBox.Text = reader.ReadToEnd();
                }
                fileOpened = true;
            }

        }

        private void saveFile_Click(object sender, EventArgs e) // On CTRL + S
        {
            Debug.WriteLine("Save file");
            

            if (string.IsNullOrEmpty(filePath))
                saveFileDialog.FileName = filePath;

            saveFileDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.  
                FileStream fs =
                   (FileStream)saveFileDialog.OpenFile();

                byte[] info = new UTF8Encoding(true).GetBytes(textBox.Text);
                fs.Write(info, 0, info.Length);

                fs.Close();
            }
        }

        

        #region Window Drag
        private void window_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void window_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }
        #endregion

    }
}
