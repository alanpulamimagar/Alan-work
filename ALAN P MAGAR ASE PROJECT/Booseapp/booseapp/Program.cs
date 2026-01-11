using System;
using System.Windows.Forms;

namespace booseapp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start your main form
            Application.Run(new Form1());
        }
    }
}
