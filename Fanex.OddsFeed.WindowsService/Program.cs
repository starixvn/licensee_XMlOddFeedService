namespace Fanex.OddsFeed.WindowsService
{
    using System;
    using System.Collections.Generic;
    using System.ServiceProcess;
    using Couchbase;
    using Couchbase.Configuration.Client;
    using Couchbase.Tracing;
    using Data.Configuration;
    using Fanex.OddsFeed.Common.Configurations;
    using Fanex.OddsFeed.WindowsServiceLibrary.Services;

    public static class Program
    {
        public static void Main()
        {
            DbSettingProviderManager.Current.Start();
            var servicesToRun = new ServiceBase[]
              {
                      new OddsFeedService()
              };

            ServiceBase.Run(servicesToRun);

            ////var config = new ClientConfiguration
            ////{
            ////    Servers = new List<Uri> { new Uri(AppSettings.CouchbaseServer) },
            ////    BucketConfigs = new Dictionary<string, BucketConfiguration> {

            ////    },
            ////};
            ////ClusterHelper.Initialize(config);

            ////var x = new WindowsOddsFeedService();
            ////x.Update();

            ////x.Update();
        }
    }
}