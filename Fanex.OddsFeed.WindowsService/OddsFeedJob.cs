namespace Fanex.OddsFeed.WindowsService
{
    using System;
    using Fanex.Logging;
    using Fanex.OddsFeed.Common.Configurations;
    using Fanex.OddsFeed.WindowsServiceLibrary.Services;
    using NLog;
    using ONEbook.UM.Services;
    using Quartz;

    public class OddsFeedJob : IJob
    {
        private readonly NLog.Logger _fileLogger = LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext context)
        {
            var startTime = DateTime.Now;
            var isSuccessful = false;

            try
            {
                var dataMap = context.JobDetail.JobDataMap;
                var _oddsFeedService = (IWindowsOddsFeedService)dataMap.Get("oddsFeedService");
                var _maintenanceService = (IMaintenanceService)dataMap.Get("maintenanceService");

                if (_maintenanceService.IsUnderMaintenance())
                {
                    _fileLogger.Trace("UM Time");
                    return;
                }

                isSuccessful = _oddsFeedService.Update();
                if (!isSuccessful)
                {
                    LogService.Error("IP address seems to have changed OR Cache on CouchBase is full. Please restart OddsFeedWindowService!");
                }
            }
            catch (Exception exception)
            {
                LogService.Error("OddsFeedJob:Execute -- " + GetExceptionMessage(exception), exception);
            }
            finally
            {
                _fileLogger.Trace("Time: " + (DateTime.Now - startTime) + ", Success: " + isSuccessful);
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