using Fanex.OddsFeed.AutoJobs.Models;

namespace Fanex.OddsFeed.AutoJobs.Helpers
{
    public static class JobHelper
    {
        public static JobSetting? GetJobConfig(IEnumerable<JobSetting> jobConfigs, string jobName)
            => jobConfigs.FirstOrDefault(job => job.JobName.Equals(jobName));
    }
}