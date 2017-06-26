namespace Fanex.OddsFeed.WindowsService
{
    using System;
    using System.ServiceProcess;
    using Common.Configurations;
    using Logging;
    using NLog;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
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