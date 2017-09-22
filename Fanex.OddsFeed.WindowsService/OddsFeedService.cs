namespace Fanex.OddsFeed.WindowsService
{
    using Common.Configurations;
    using Couchbase;
    using Couchbase.Configuration.Client;
    using Logging;
    using NLog;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
    using System;
    using System.Collections.Generic;
    using System.ServiceProcess;
    using WindowsServiceLibrary.Services;

    public partial class OddsFeedService : ServiceBase
    {
        private readonly IWindowsOddsFeedService _oddsFeedService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly JobScheduler _scheduler;
        private readonly NLog.Logger _fileLogger;

        public OddsFeedService()
        {
            InitializeComponent();
            _oddsFeedService = new WindowsOddsFeedService();
            _maintenanceService = new MaintenanceService();
            _scheduler = new JobScheduler();
            _fileLogger = LogManager.GetCurrentClassLogger();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                ClusterHelper.Initialize(new ClientConfiguration
                {
                    Servers = new List<Uri> { new Uri(AppSettings.CouchbaseServer) }
                });
                _fileLogger.Trace("OddsFeedService: OnStart");
                _oddsFeedService.Reset();
                _scheduler.Start(_oddsFeedService, _maintenanceService);
            }
            catch (Exception exception)
            {
                LogService.Error("OnStart -- " + GetExceptionMessage(exception));
            }
        }

        protected override void OnStop()
        {
            try
            {
                ClusterHelper.Close();
                _fileLogger.Trace("OddsFeedService: OnStop");
                _scheduler.Stop();
            }
            catch (Exception exception)
            {
                LogService.Error("OnStop -- " + GetExceptionMessage(exception));
            }
        }

        private static string GetExceptionMessage(Exception exception)
        {
            var messages = new string[]
            {
                "ENVIRONMENT: " + AppSettings.Environment,
                "ERROR: " + exception.Message,
                "SOURCE: " + exception.Source,
                "TARGETSITE: " + exception.TargetSite,
                "STACKTRACE: " + exception.StackTrace
            };

            return string.Join(Environment.NewLine, messages);
        }
    }
}