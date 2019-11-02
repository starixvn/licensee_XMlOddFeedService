namespace Fanex.OddsFeed.WindowsService
{
    using System;
    using System.Collections.Generic;
    using System.ServiceProcess;
    using Common.Configurations;
    using Couchbase;
    using Couchbase.Configuration.Client;
    using Fanex.Logging;
    using NLog;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
    using WindowsServiceLibrary.Services;

    public partial class OddsFeedService : ServiceBase
    {
        private readonly IWindowsOddsFeedService oddsFeedService;
        private readonly IMaintenanceService maintenanceService;
        private readonly JobScheduler scheduler;
        private readonly NLog.Logger fileLogger;

        public OddsFeedService()
        {
            InitializeComponent();
            oddsFeedService = new WindowsOddsFeedService();
            maintenanceService = new MaintenanceService();
            scheduler = new JobScheduler();
            fileLogger = LogManager.GetCurrentClassLogger();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                ClusterHelper.Initialize(new ClientConfiguration
                {
                    Servers = new List<Uri> { new Uri(AppSettings.CouchbaseServer) }
                });
                fileLogger.Trace("OddsFeedService: OnStart");
                oddsFeedService.Reset();
                scheduler.Start(oddsFeedService, maintenanceService);
            }
            catch (Exception exception)
            {
                LogService.Error("OnStart -- " + oddsFeedService.GetExceptionMessage(exception));
            }
        }

        protected override void OnStop()
        {
            try
            {
                ClusterHelper.Close();
                fileLogger.Trace("OddsFeedService: OnStop");
                scheduler.Stop();
            }
            catch (Exception exception)
            {
                LogService.Error("OnStop -- " + oddsFeedService.GetExceptionMessage(exception));
            }
        }
    }
}