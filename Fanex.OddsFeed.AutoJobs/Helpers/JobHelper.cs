namespace Fanex.OddsFeed.AutoJobs.Helpers
{
    using Fanex.OddsFeed.AutoJobs.Models;

    public static class JobHelper
    {
        public static JobSetting? GetJobConfig(IEnumerable<JobSetting> jobConfigs, string jobName)
            => jobConfigs.FirstOrDefault(job => job.JobName.Equals(jobName));
    }
}