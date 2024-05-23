namespace OddsFeed.AutoJobs.Workers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using OddsFeed.AutoJobs.Models;
    using OddsFeed.Core.Services;
    using ONEbook.UM.Services;

    public abstract class BaseWorker : BackgroundService
    {
        private readonly IMaintenanceService maintenanceService;
        private readonly ILogServiceProvider logService;
        protected JobSetting jobSetting;

        protected BaseWorker(ILogServiceProvider logService, IMaintenanceService maintenanceService)
        {
            this.logService = logService;
            this.maintenanceService = maintenanceService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const byte MillisecondsDelay = 10;
            await Task.Delay(MillisecondsDelay);

            while (!stoppingToken.IsCancellationRequested)
            {
                logService.LogInfoToFile($"{jobSetting.JobName} start run");
                await RunJobAsync();
                logService.LogInfoToFile($"{jobSetting.JobName} end run");
                await Task.Delay(TimeSpan.FromSeconds(jobSetting.TimeInterval), stoppingToken);
            }

            logService.LogInfoToFile($"Execution ended. Cancelation token cancelled = {stoppingToken.IsCancellationRequested}");
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logService.LogInfoToFile($"{jobSetting.JobName}: StartAsync");
            if (jobSetting.HasStartFunction)
            {
                logService.LogInfoToFile($"{jobSetting.JobName}: Start Function Run");
                StartJobAsync();
            }

            return base.StartAsync(cancellationToken);
        }

        public virtual async Task RunJobAsync()
        {
            try
            {
                if (jobSetting.IsEnabled && (!jobSetting.HasCheckUM || !IsUM()) && !IsDisableAtWeekend())
                {
                    await ExecuteJobAsync();
                }
            }
            catch (Exception ex)
            {
                await logService.LogSentryAsync(ex);
            }
        }

        public abstract Task ExecuteJobAsync();

        public abstract Task StartJobAsync();

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logService.LogInfoToFile($"{jobSetting.JobName}: Stopping Service");

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            logService.LogInfoToFile($"{jobSetting.JobName}: Disposing Service");
            base.Dispose();
        }

        private bool IsUM()
        {
            var maintanceTime = maintenanceService.GetUnderMaintenanceTime();
            return maintanceTime.IsUnderMaintenanceTime;
        }

        private bool IsDisableAtWeekend()
        {
            return jobSetting.IsDisableAtWeekend &&
                (DateTimeOffset.Now.DayOfWeek == DayOfWeek.Thursday || DateTimeOffset.Now.DayOfWeek == DayOfWeek.Sunday);
        }
    }
}