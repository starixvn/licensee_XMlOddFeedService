
namespace Fanex.OddsFeed.WindowsService
{
    using Fanex.OddsFeed.Common.Configurations;
    using Fanex.OddsFeed.WindowsServiceLibrary.Services;
    using ONEbook.UM.Services;
    using Quartz;
    using Quartz.Impl;
    using System.Collections.Generic;

    public class JobScheduler
    {
        private readonly int interval;
        private readonly IScheduler scheduler;

        public JobScheduler()
        {
            interval = AppSettings.Interval;
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
        }

        public void Start(IWindowsOddsFeedService oddsFeedService, IMaintenanceService maintenanceService)
        {
            var job = JobBuilder.Create<OddsFeedJob>()
                .UsingJobData(new JobDataMap(new Dictionary<string, IWindowsOddsFeedService>
                {
                    { nameof(oddsFeedService), oddsFeedService}
                }))
                .UsingJobData(new JobDataMap(new Dictionary<string, IMaintenanceService>
                {
                    { nameof(maintenanceService), maintenanceService}
                }))
                .Build();

            var trigger = TriggerBuilder.Create()
                   .WithSimpleSchedule(a => a.WithIntervalInSeconds(interval)
                   .RepeatForever())
                   .Build();

            scheduler.ScheduleJob(job, trigger);
            scheduler.Start();
        }

        public void Stop()
        {
            scheduler.Shutdown();
        }
    }
}