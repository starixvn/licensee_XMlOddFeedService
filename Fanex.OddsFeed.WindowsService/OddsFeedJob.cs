namespace Fanex.OddsFeed.WindowsService
{
    using Fanex.Logging;
    using Fanex.OddsFeed.Common.Configurations;
    using Fanex.OddsFeed.WindowsServiceLibrary.Services;
    using NLog;
    using ONEbook.UM.Services;
    using Quartz;
    using System;

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

                if (_maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime)
                {
                    _fileLogger.Trace("UM Time");
                    return;
                }

                isSuccessful = _oddsFeedService.Update();
            }
            catch (Exception exception)
            {
                LogService.Error("OddsFeedJob:Execute -- " + GetExceptionMessage(exception), exception);
                _fileLogger.Trace("Exception: " + GetExceptionMessage(exception));
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