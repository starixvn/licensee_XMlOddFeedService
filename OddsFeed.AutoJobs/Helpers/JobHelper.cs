namespace OddsFeed.AutoJobs.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using OddsFeed.AutoJobs.Models;

    public static class JobHelper
    {
        public static JobSetting GetJobConfig(IEnumerable<JobSetting> jobConfigs, string jobName)
            => jobConfigs.FirstOrDefault(job => job.JobName.Equals(jobName));
    }
}