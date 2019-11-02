namespace Fanex.OddsFeed.WindowsService
{
    using System;
    using Fanex.Logging;
    using Fanex.OddsFeed.WindowsServiceLibrary.Services;
    using NLog;
    using ONEbook.UM.Services;
    using Quartz;

    public class OddsFeedJob : IJob
    {
        private readonly NLog.Logger fileLogger = LogManager.GetCurrentClassLogger();
        private IWindowsOddsFeedService oddsFeedService;
        private static bool isReady = true;
        private readonly DateTime dateTime = DateTime.Now;

        public void Execute(IJobExecutionContext context)
        {
            fileLogger.Trace(dateTime);
            if (isReady)
            {
                isReady = false;
                var startTime = DateTime.Now;
                var isSuccessful = false;

                try
                {
                    var dataMap = context.JobDetail.JobDataMap;
                    var maintenanceService = (IMaintenanceService)dataMap.Get("maintenanceService");
                    oddsFeedService = (IWindowsOddsFeedService)dataMap.Get("oddsFeedService");

                    if (maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime)
                    {
                        fileLogger.Trace("UM Time");
                        isReady = true;
                        return;
                    }

                    isSuccessful = oddsFeedService.Update();
                }
                catch (Exception exception)
                {
                    LogService.Error("OddsFeedJob:Execute -- " + oddsFeedService.GetExceptionMessage(exception), exception);
                }
                finally
                {
                    fileLogger.Trace("Time: " + (DateTime.Now - startTime) + ", Success: " + isSuccessful);
                    isReady = true;
                }
            }
            else
            {
                fileLogger.Trace("Previous call has not finished yet.");
            }
        }
    }
}