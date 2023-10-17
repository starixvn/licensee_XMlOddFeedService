namespace Fanex.OddsFeed.Core.Configurations
{
    using System;

    [Serializable]
    public class AppSettings
    {
        public string CachePrefix { get; set; }
        public string BetTypeSchemaPath { get; set; }
        public string BetTypeSchemaKey { get; set; }
        public Bucket Bucket { get; set; }
        public ResultFeed ResultFeed { get; set; }
    }

    [Serializable]
    public class Bucket
    {
        public int TotalVersion { get; set; }
        public string OddsFeed { get; set; }
        public string Outrights { get; set; }
        public string LiveMatches { get; set; }
        public string DeadMatches { get; set; }
        public string Throttler { get; set; }
        public string BlockList { get; set; }
    }

    [Serializable]
    public class ResultFeed
    {
        public int TotalVersion { get; set; }
        public string Bucket { get; set; }
        public string BucketData { get; set; }
    }
}