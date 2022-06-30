namespace Fanex.OddsFeed.WindowsService
{
    using System;
    using Fanex.Logging;
    using Fanex.OddsFeed.Common.Configurations;
    using Fanex.OddsFeed.WindowsServiceLibrary.Services;
    using ONEbook.UM.Services;
    using Quartz;

    public class ResultFeedJob : IJob
    {
        private readonly NLog.Logger fileLogger = NLog.LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext context)
        {
            var startTime = DateTime.Now;
            var isSuccessful = false;

            try
            {
                var dataMap = context.JobDetail.JobDataMap;
                var resultFeedService = (IResultFeedService)dataMap.Get("resultFeedService");
                var maintenanceService = (IMaintenanceService)dataMap.Get("maintenanceService");

                if (maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime)
                {
                    fileLogger.Trace("UM Time");
                    return;
                }

                isSuccessful = resultFeedService.Update();
            }
            catch (Exception exception)
            {
                LogService.Error("OddsFeedJob:Execute -- " + GetExceptionMessage(exception), exception);
                fileLogger.Trace("Exception: " + GetExceptionMessage(exception));
            }
            finally
            {
                fileLogger.Trace("Time: " + (DateTime.Now - startTime) + ", Success: " + isSuccessful);
            }
        }

        private static string GetExceptionMessage(Exception exception)
        {
            var innerException = (exception.InnerException != null) ? exception.InnerException.Message + " " + exception.InnerException.StackTrace : string.Empty;
            var messages = new[]
            {
                "ENVIRONMENT: " + AppSettings.Environment,
                "ERROR: " + exception.Message,
                "SOURCE: " + exception.Source,
                "TARGETSITE: " + exception.TargetSite,
                "STACKTRACE: " + exception.StackTrace,
                "INNER EXCEPTION: " + innerException
            };

            return string.Join(Environment.NewLine, messages);
        }
    }
}