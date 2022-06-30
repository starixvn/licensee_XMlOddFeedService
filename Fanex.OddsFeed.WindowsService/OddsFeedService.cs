namespace Fanex.OddsFeed.WindowsService
{
    using System;
    using System.ServiceProcess;
    using Common.Configurations;
    using Logging;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
    using WindowsServiceLibrary.Services;

    public partial class OddsFeedService : ServiceBase
    {
        private readonly IWindowsOddsFeedService oddsFeedService;
        private readonly IResultFeedService resultFeedService;
        private readonly IMaintenanceService maintenanceService;
        private readonly JobScheduler scheduler;
        private readonly NLog.Logger fileLogger;

        public OddsFeedService()
        {
            InitializeComponent();
            oddsFeedService = new WindowsOddsFeedService();
            resultFeedService = new ResultFeedService();
            maintenanceService = new MaintenanceService();
            scheduler = new JobScheduler();
            fileLogger = NLog.LogManager.GetCurrentClassLogger();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                fileLogger.Trace("OddsFeedService: OnStart");
                oddsFeedService.Reset();
                resultFeedService.Reset();
                scheduler.Start(oddsFeedService, resultFeedService, maintenanceService);
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
                fileLogger.Trace("OddsFeedService: OnStop");
                scheduler.Stop();
            }
            catch (Exception exception)
            {
                LogService.Error("OnStop -- " + GetExceptionMessage(exception));
            }
        }

        private static string GetExceptionMessage(Exception exception)
        {
            var messages = new[]
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