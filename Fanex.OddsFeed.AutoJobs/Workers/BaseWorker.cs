namespace Fanex.OddsFeed.AutoJobs.Workers
{
    using Fanex.OddsFeed.AutoJobs.Models;
    using Fanex.OddsFeed.Core.Services;
    using Microsoft.Extensions.Hosting;
    using ONEbook.UM.Services;

    public abstract class BaseWorker : BackgroundService
    {
        private readonly IMaintenanceService maintenanceService;
        private readonly ILogServiceProvider logService;
        protected JobSetting jobSetting;

        public BaseWorker(ILogServiceProvider logService, IMaintenanceService maintenanceService)
        {
            this.logService = logService;
            this.maintenanceService = maintenanceService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const byte MillisecondsDelay = 10;
            await Task.Delay(MillisecondsDelay).ConfigureAwait(false);

            while (!stoppingToken.IsCancellationRequested)
            {
                logService.LogFile($"{jobSetting.JobName} start run");
                await RunJobAsync();
                logService.LogFile($"{jobSetting.JobName} end run");
                await Task.Delay(TimeSpan.FromSeconds(jobSetting.TimeInterval), stoppingToken);
            }

            logService.LogFile($"Execution ended. Cancelation token cancelled = {stoppingToken.IsCancellationRequested}");
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logService.LogFile($"{jobSetting.JobName}: StartAsync");
            if (jobSetting.HasStartFunction)
            {
                logService.LogFile($"{jobSetting.JobName}: Start Function Run");
                StartJobAsync();
            }

            return base.StartAsync(cancellationToken);
        }

        public virtual async Task RunJobAsync()
        {
            try
            {
                if (jobSetting.IsEnabled && (!jobSetting.HasCheckUM || !IsUM()))
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
            logService.LogFile($"{jobSetting.JobName}: Stopping Service");

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            logService.LogFile($"{jobSetting.JobName}: Disposing Service");
            base.Dispose();
        }

        private bool IsUM()
        {
            var maintanceTime = maintenanceService.GetUnderMaintenanceTime();
            return maintanceTime.IsUnderMaintenanceTime;
        }
    }
}