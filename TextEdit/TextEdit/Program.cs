using System;
using System.Windows.Forms;

namespace TextEdit
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string newFile = "";
            if (args.Length != 0) // if application is opened with external file
            {
                newFile = args[0];
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new form(newFile));
        }
    }
}
