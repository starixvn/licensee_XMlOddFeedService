using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Couchbase;
using Couchbase.Configuration.Client;
using Fanex.OddsFeed.Common.Configurations;
using Fanex.OddsFeed.Common.DTO;
using Newtonsoft.Json;

namespace CouchbaseApp
{
    public partial class Form1 : Form
    {
        private readonly Cluster _cluster;

        public Form1()
        {
            InitializeComponent();

            _cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri(AppSettings.CouchbaseServer) }
            });
        }

        private void btnVersionFullChain_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbVersionFullchain.Text))
            {
                MessageBox.Show("Please select Version/Fullchain key!");
                return;
            }

            if (cbVersionFullchain.Text.Equals("apollocurrent", StringComparison.InvariantCultureIgnoreCase))
            {
                Print<int>(AppSettings.BucketOddsFeed, CacheKeys.CurrentKey(AppSettings.CachePrefix));
            }
            else
            {
                Print<bool>(AppSettings.BucketOddsFeed, CacheKeys.FullChainKey(AppSettings.CachePrefix));
            }
        }

        private void Print<T>(string bucketName, string key)
        {
            using (var bucket = _cluster.OpenBucket(bucketName))
            {
                var data = bucket.Get<T>(key);

                textBox1.Text = data.Value.ToString();
            }
        }

        private void btnMatchOutright_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbMatchOutright.Text))
            {
                MessageBox.Show("Please select Match/Outright key!");
                return;
            }

            var version = string.IsNullOrEmpty(cbNumberVersion.Text) ? "0" : cbNumberVersion.Text;

            switch (cbMatchOutright.Text)
            {
                case "apolloevents":
                    HandleData<IList<Outright>>(AppSettings.BucketOutrights, version);
                    break;
                case "apollolivematchs":
                    HandleData<IList<Match>>(AppSettings.BucketLiveMatches, version);
                    break;
                case "apollodeadballmatches":
                    HandleData<IList<Match>>(AppSettings.BucketDeadMatches, version);
                    break;
            }
        }

        private void HandleData<T>(string bucketName, string key)
        {
            using (var bucket = _cluster.OpenBucket(bucketName))
            {
                var result = bucket.Get<T>(key);
                if (result.Success)
                {
                    ParseToForm(result.Value);
                }
            }
        }

        private void ParseToForm<T>(T data)
        {
            if (data != null)
            {
                textBox1.Text = JsonConvert.SerializeObject(data);
            }
            else
            {
                textBox1.Text = string.Empty;
            }
        }
    }
}
