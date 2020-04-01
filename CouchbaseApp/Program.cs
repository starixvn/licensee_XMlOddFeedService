using System;
using System.Windows.Forms;

namespace CouchbaseApp
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            //DbSettingProviderManager.Current.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var form = new Form1())
            {
                Application.Run(form);
            }
        }
    }
}