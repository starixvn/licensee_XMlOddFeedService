using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fanex.OddsFeed.Common.Configurations;
using Fanex.OddsFeed.WindowsServiceLibrary.Services;

namespace CouchbaseApp
{
    public partial class LogFormcs : Form
    {
        private IWindowsOddsFeedService _oddsFeedService;

        public LogFormcs()
        {
            InitializeComponent();
        }

        private void RequestSubmit_Click(object sender, EventArgs e)
        {
            //_oddsFeedService = new OldWindowsOddsFeedService();
            OldResultView.Items.Clear();
            Request(OldResultView, false);
        }

        private void NewRequestSubmit_Click(object sender, EventArgs e)
        {
            _oddsFeedService = new WindowsOddsFeedService();
            NewResultView.Items.Clear();
            Request(NewResultView, true);
        }

        private void Request(ListView view, bool isNew)
        {
            var counter = 0;
            var numberRequest = Convert.ToInt32(RequestTimes.Text);

            do
            {
                counter++;

                if (_oddsFeedService.Update())
                {
                    //view.Items.Add(new ListViewItem(new string[] 
                    //{
                    //    counter.ToString(),
                    //    $"{_oddsFeedService.LiveballTime}",
                    //    $"{_oddsFeedService.DeadballTime}",
                    //    $"{_oddsFeedService.OutrightTime}",
                    //    $"{_oddsFeedService.FullTime}"
                    //}));
                }

                Thread.Sleep(AppSettings.Interval);
            }
            while (counter < numberRequest);
        }
    }
}
