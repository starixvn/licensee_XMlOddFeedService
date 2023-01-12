namespace Fanex.OddsFeed.Core.Configurations
{
    public class AppSettings
    {
        public int MaxTimeRangeLive { get; set; }
        public int MaxTimeRangeDeadball { get; set; }
        public int MaxTimeRangeOutright { get; set; }
        public string CachePrefix { get; set; } = string.Empty;
        public string BetTypeSchemaPath { get; set; } = string.Empty;
        public string BetTypeSchemaKey { get; set; } = string.Empty;
        public int TotalVersion { get; set; }
        public Bucket Bucket { get; set; }
        public ResultFeed ResultFeed { get; set; }
    }

    public class Bucket
    {
        public string OddsFeed { get; set; } = string.Empty;
        public string Outrights { get; set; } = string.Empty;
        public string LiveMatches { get; set; } = string.Empty;
        public string DeadMatches { get; set; } = string.Empty;
    }

    public class ResultFeed
    {
        public int TotalVersion { get; set; }
        public string Bucket { get; set; } = string.Empty;
        public string BucketData { get; set; } = string.Empty;
    }
}