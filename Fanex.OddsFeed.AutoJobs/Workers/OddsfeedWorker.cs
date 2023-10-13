namespace Fanex.OddsFeed.AutoJobs.Workers
{
    using Fanex.OddsFeed.AutoJobs.Constants;
    using Fanex.OddsFeed.AutoJobs.Helpers;
    using Fanex.OddsFeed.AutoJobs.Models;
    using Fanex.OddsFeed.AutoJobs.Services;
    using Fanex.OddsFeed.Core.Services;
    using ONEbook.UM.Services;

    public class OddsfeedWorker : BaseWorker
    {
        private readonly IOddsfeedService oddsfeedService;

        public OddsfeedWorker(IOddsfeedService oddsfeedService,
            List<JobSetting> jobSettings,
            ILogServiceProvider logService,
            IMaintenanceService maintenanceService)
            : base(logService, maintenanceService)
        {
            this.oddsfeedService = oddsfeedService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobNames.OddsfeedUpdateJob);
        }

        public override async Task ExecuteJobAsync()
        {
            await oddsfeedService.Update();
        }

        public override async Task StartJobAsync()
        {
            await oddsfeedService.Reset();
        }
    }
}