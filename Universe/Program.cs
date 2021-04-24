using System;
using System.Windows.Forms;
using Universe.Forms;

namespace Universe
{
    public static class Program
    {
        public static FormMain formMain;

        //        public static FormMain FormMain { get => formMain; set => formMain = value; }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (FormMain formMain = new FormMain())
            {
                Application.Run(formMain);
            }
        }
    }
}
