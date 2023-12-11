namespace OddsFeed.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using OddsFeed.AutoJobs.Constants;
    using OddsFeed.AutoJobs.Helpers;
    using OddsFeed.AutoJobs.Models;
    using OddsFeed.AutoJobs.Services;
    using OddsFeed.Core.Services;
    using ONEbook.UM.Services;

    public class ResultFeedWorker : BaseWorker
    {
        private readonly IResultFeedService oddsfeedService;

        public ResultFeedWorker(IResultFeedService oddsfeedService,
            List<JobSetting> jobSettings,
            ILogServiceProvider logService,
            IMaintenanceService maintenanceService)
            : base(logService, maintenanceService)
        {
            this.oddsfeedService = oddsfeedService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobNames.ResultFeedUpdateJob);
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