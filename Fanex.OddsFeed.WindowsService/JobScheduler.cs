namespace Fanex.OddsFeed.WindowsService
{
    using System.Collections.Generic;
    using Fanex.OddsFeed.Common.Configurations;
    using Fanex.OddsFeed.WindowsServiceLibrary.Services;
    using ONEbook.UM.Services;
    using Quartz;
    using Quartz.Impl;

    public class JobScheduler
    {
        private readonly int oddsInterval;
        private readonly int resultInterval;
        private readonly IScheduler scheduler;

        public JobScheduler()
        {
            oddsInterval = AppSettings.Interval;
            resultInterval = AppSettings.ResultFeed_Interval;
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
        }

        public void Start(IWindowsOddsFeedService oddsFeedService, IResultFeedService resultFeedService, IMaintenanceService maintenanceService)
        {
            // odds
            var oddsjob = JobBuilder.Create<OddsFeedJob>()
                .UsingJobData(new JobDataMap(new Dictionary<string, IWindowsOddsFeedService>
                {
                    { nameof(oddsFeedService), oddsFeedService}
                }))
                .UsingJobData(new JobDataMap(new Dictionary<string, IMaintenanceService>
                {
                    { nameof(maintenanceService), maintenanceService}
                }))
                .Build();

            var oddstrigger = TriggerBuilder.Create()
                   .WithSimpleSchedule(a => a.WithIntervalInSeconds(oddsInterval)
                   .RepeatForever())
                   .Build();

            // result
            var resultjob = JobBuilder.Create<ResultFeedJob>()
                .UsingJobData(new JobDataMap(new Dictionary<string, IResultFeedService>
                {
                    { nameof(resultFeedService), resultFeedService}
                }))
                .UsingJobData(new JobDataMap(new Dictionary<string, IMaintenanceService>
                {
                    { nameof(maintenanceService), maintenanceService}
                }))
                .Build();

            var resulttrigger = TriggerBuilder.Create()
                   .WithSimpleSchedule(a => a.WithIntervalInSeconds(resultInterval)
                   .RepeatForever())
                   .Build();

            scheduler.ScheduleJob(oddsjob, oddstrigger);
            scheduler.ScheduleJob(resultjob, resulttrigger);

            scheduler.Start();
        }

        public void Stop()
        {
            scheduler.Shutdown();
        }
    }
}