using Fanex.OddsFeed.Common.Configurations;
using Fanex.OddsFeed.WindowsServiceLibrary.Services;
using ONEbook.UM.Services;
using Quartz;
using Quartz.Impl;
using System.Collections.Generic;

namespace Fanex.OddsFeed.WindowsService
{
    public class JobScheduler
    {
        private readonly int _interval;
        private readonly IScheduler _scheduler;

        public JobScheduler()
        {
            _interval = AppSettings.Interval;
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();
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
                   .WithSimpleSchedule(a => a.WithIntervalInSeconds(_interval)
                   .RepeatForever())
                   .Build();

            _scheduler.ScheduleJob(job, trigger);
            _scheduler.Start();
        }

        public void Stop()
        {
            _scheduler.Shutdown();
        }
    }
}