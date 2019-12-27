using System;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace TextEdit
{
    public partial class form : Form
    {
        string temp;
        bool mouseDown = false;
        Point lastLocation;
        string filePath;
        string fileName = "New File";
        bool fileOpened = false;
        bool isExitSafe = true;
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;

        ///
        /// Handling the window messages
        ///
        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg == WM_NCHITTEST && (int)message.Result == HTCLIENT)
                message.Result = (IntPtr)HTCAPTION;
        }

        public form(string newFile) // On form startup
        {
            InitializeComponent();
            textBox.Font = Properties.Settings.Default.savedFont;
            textBox.BackColor = Properties.Settings.Default.backColor;
            if (newFile != "") // If app is opened with a text file
            {
                Debug.WriteLine("File opened on startup");

                var fileStream = newFile;

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    textBox.Text = reader.ReadToEnd();
                }

                filePath = newFile;
                fileOpened = true;
                isExitSafe = true;
                temp = textBox.Text;
            fileName = Path.GetFileName(filePath);
            }
            formName.Text = string.Format("SynText | {0}", fileName);
            RPCHandler.Awaken();
            RPCHandler.SetPresence(new DiscordRPC.RichPresence()
            {
                Details = fileName,
                Assets = new DiscordRPC.Assets()
                {
                    LargeImageKey = "cover",
                    LargeImageText = "SynText",
                }
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isExitSafe)
                QuitApplication();
            else
            {
                DialogResult saveError = MessageBox.Show("This file has been edited, and has not been saved. Would you like to save your work before closing?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (saveError == DialogResult.Yes)
                {
                    Debug.WriteLine("Save file");
                    if (!fileOpened)
                    {
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
                            isExitSafe = true;
                            QuitApplication();

                        }
                    }

                    else if (fileOpened && File.Exists(filePath)) //if user has file opened in program
                    {
                        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Write))
                        {

                            fs.SetLength(0);

                            byte[] info = new UTF8Encoding(true).GetBytes(textBox.Text);
                            fs.Write(info, 0, info.Length);

                            fs.Close();
                            formName.Text = string.Format("Text Editor | {0}", fileName);
                            isExitSafe = true;
                            QuitApplication();
                        }
                    }
                }
                else if (saveError == DialogResult.No)
                {
                    QuitApplication();
                }
            }
        }

        private void openFile_Click(object sender, EventArgs e) // On CTRL + O
        {
            Debug.WriteLine("Open file");

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                filePath = openFileDialog.FileName;
                fileName = Path.GetFileName(filePath);
                //Read the contents of the file into a stream
                var fileStream = openFileDialog.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    textBox.Text = reader.ReadToEnd();
                }
                fileOpened = true;
                formName.Text = string.Format("Text Editor | {0}", fileName);

            }

        }

        private void saveFile_Click(object sender, EventArgs e) // On CTRL + S
        {
            Debug.WriteLine("Save file");
            if (!fileOpened)
            {
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
                    isExitSafe = true;
                    temp = textBox.Text;
                }
            }

            else if (fileOpened && File.Exists(filePath)) //if user has file opened in program
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Write))
                {

                    fs.SetLength(0);

                    byte[] info = new UTF8Encoding(true).GetBytes(textBox.Text);
                    fs.Write(info, 0, info.Length);

                    fs.Close();
                    formName.Text = string.Format("Text Editor | {0}", fileName);
                    isExitSafe = true;
                }
            }

        }

        private void QuitApplication()
        {
            Properties.Settings.Default.Save();

            Application.Exit();
        }


        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Save file as new");
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
                isExitSafe = true;
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
                this.WindowState = FormWindowState.Normal;

                this.Update();
            }
        }

        private void window_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
        #endregion

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                textBox.Font = fontDialog.Font;
                Properties.Settings.Default.savedFont = fontDialog.Font;
            }
        }

        private void Minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void toggleFullScreen_Click(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Maximized)
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Normal;

        }


        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                textBox.BackColor = colorDialog.Color;
                Properties.Settings.Default.backColor = colorDialog.Color;
            }
        }
        private void timeStampToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Text += $"!!TIMESTAMP!!";
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (temp != textBox.Text)
                isExitSafe = false; // Prevents application closing if file has been edited without saving
            else
                isExitSafe = true;
            textBox.Text = textBox.Text.Replace("!!TIMESTAMP!!", $"--- {DateTime.Now.ToString()} ---{Environment.NewLine}");
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
    }
}

