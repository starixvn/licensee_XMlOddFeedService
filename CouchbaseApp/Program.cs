using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fanex.Data.Configuration;

namespace CouchbaseApp
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
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
