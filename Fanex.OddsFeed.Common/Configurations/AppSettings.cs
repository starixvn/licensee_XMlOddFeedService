namespace Fanex.OddsFeed.Common.Configurations
{
    using System;
    using System.Configuration;

    public static class AppSettings
    {
        public static string FeedUrlPath => ConfigurationManager.AppSettings["FeedUrlPath"];

        public static string VirtualPathWebService => ConfigurationManager.AppSettings["VirtualPathWebService"];

        public static string Environment => ConfigurationManager.AppSettings["Environment"];

        public static int TotalVersion => Convert.ToInt32(ConfigurationManager.AppSettings["TotalVersion"]);

        public static int Interval => Convert.ToInt32(ConfigurationManager.AppSettings["Interval"]);

        public static bool IsSpLive => Convert.ToBoolean(ConfigurationManager.AppSettings["SP.Live"]);

        public static string SpSuffix => ConfigurationManager.AppSettings["SP.Suffix"];

        public static string SmtpClientHost => ConfigurationManager.AppSettings["SmtpClient.Host"];

        public static bool SmtpClientEnableSsl => Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpClient.EnableSsl"]);

        public static int SmtpClientPort => Convert.ToInt32(ConfigurationManager.AppSettings["SmtpClient.Port"]);

        public static string SmtpClientAccount => ConfigurationManager.AppSettings["SmtpClient.Account"];

        public static string SmtpClientPassword => ConfigurationManager.AppSettings["SmtpClient.Password"];

        public static string SpTestingUMTime => ConfigurationManager.AppSettings["SPTestingUMTime"];

        public static bool IsDisableUMTimeFlag => Convert.ToBoolean(ConfigurationManager.AppSettings["IsDisableUMTimeFlag"]);

        public static int LimitedOptions => Convert.ToInt32(ConfigurationManager.AppSettings["LimitedOptions"]);

        public static string HighlightSportType => ConfigurationManager.AppSettings["HighlightSportType"];

        public static string HighlightBetType => ConfigurationManager.AppSettings["HighlightBetType"];

        public static string BetTypeSchemaPath => ConfigurationManager.AppSettings["BetTypeSchemaPath"];

        public static string BetTypeDescriptionPath => ConfigurationManager.AppSettings["BetTypeDescriptionPath"];

        public static string BetTypeSchemaKey => ConfigurationManager.AppSettings["BetTypeSchemaKey"];

        public static string CachePrefix => ConfigurationManager.AppSettings["Cache.Prefix"];

        public static int NumberTries => Convert.ToInt32(ConfigurationManager.AppSettings["NumTries"]);

        public static string FromMail => ConfigurationManager.AppSettings["FromMail"];

        public static string CouchbaseServer => ConfigurationManager.AppSettings["CouchbaseServer"];

        public static string BucketOddsFeed => ConfigurationManager.AppSettings["Bucket.OddsFeed"];

        public static string BucketOutrights => ConfigurationManager.AppSettings["Bucket.Outrights"];

        public static string BucketLiveMatches => ConfigurationManager.AppSettings["Bucket.LiveMatches"];

        public static string BucketDeadMatches => ConfigurationManager.AppSettings["Bucket.DeadMatches"];
    }
}