namespace Fanex.OddsFeed.Core.Configurations
{
    public class AppSettings
    {
        public int MaxTimeRangeLive { get; set; }
        public int MaxTimeRangeDeadball { get; set; }
        public int MaxTimeRangeOutright { get; set; }
        public string CachePrefix { get; set; }
        public string BetTypeSchemaPath { get; set; }
        public string BetTypeSchemaKey { get; set; }
        public int Interval { get; set; }
        public int TotalVersion { get; set; }
        public Bucket Bucket { get; set; }
        public ResultFeed ResultFeed { get; set; }
    }

    public class Bucket
    {
        public string OddsFeed { get; set; }
        public string Outrights { get; set; }
        public string LiveMatches { get; set; }
        public string DeadMatches { get; set; }
        public string Throttler { get; set; }
        public string BlockList { get; set; }
    }

    public class ResultFeed
    {
        public int TotalVersion { get; set; }
        public string Bucket { get; set; }
        public string BucketData { get; set; }
        public int Interval { get; set; }
    }
}