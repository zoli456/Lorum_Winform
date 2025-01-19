using System;
using System.Windows.Forms;

namespace Lórum_Winform
{
    public class Program
    {
        public static Form1 Mainform { get; private set; }

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Mainform = new Form1();
            Application.Run(Mainform);
        }
    }
}