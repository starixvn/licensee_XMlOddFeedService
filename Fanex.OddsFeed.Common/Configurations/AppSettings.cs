namespace Fanex.OddsFeed.Common.Configurations
{
    using System;
    using System.Configuration;

    public static class AppSettings
    {
        public static string FeedUrlPath
        {
            get
            {
                return ConfigurationManager.AppSettings["FeedUrlPath"].ToString();
            }
        }

        public static string VirtualPathWebService
        {
            get
            {
                return ConfigurationManager.AppSettings["VirtualPathWebService"].ToString();
            }
        }

        public static string Environment
        {
            get
            {
                return ConfigurationManager.AppSettings["Environment"].ToString();
            }
        }

        public static int TotalVersion
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["TotalVersion"]);
            }
        }

        public static int Interval
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["Interval"]);
            }
        }

        public static bool IsSpLive
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["SP.Live"]);
            }
        }

        public static string SpSuffix
        {
            get
            {
                return ConfigurationManager.AppSettings["SP.Suffix"].ToString();
            }
        }

        public static string SmtpClientHost
        {
            get
            {
                return ConfigurationManager.AppSettings["SmtpClient.Host"].ToString();
            }
        }

        public static bool SmtpClientEnableSsl
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpClient.EnableSsl"]);
            }
        }

        public static int SmtpClientPort
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["SmtpClient.Port"]);
            }
        }

        public static string SmtpClientAccount
        {
            get
            {
                return ConfigurationManager.AppSettings["SmtpClient.Account"].ToString();
            }
        }

        public static string SmtpClientPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["SmtpClient.Password"].ToString();
            }
        }

        public static string SpTestingUMTime
        {
            get
            {
                return ConfigurationManager.AppSettings["SPTestingUMTime"].ToString();
            }
        }

        public static bool IsDisableUMTimeFlag
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["IsDisableUMTimeFlag"]);
            }
        }

        public static int LimitedOptions
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["LimitedOptions"]);
            }
        }

        public static string HighlightSportType
        {
            get
            {
                return ConfigurationManager.AppSettings["HighlightSportType"].ToString();
            }
        }

        public static string HighlightBetType
        {
            get
            {
                return ConfigurationManager.AppSettings["HighlightBetType"].ToString();
            }
        }

        public static string BetTypeSchemaPath
        {
            get
            {
                return ConfigurationManager.AppSettings["BetTypeSchemaPath"].ToString();
            }
        }

        public static string BetTypeDescriptionPath
        {
            get
            {
                return ConfigurationManager.AppSettings["BetTypeDescriptionPath"].ToString();
            }
        }

        public static string BetTypeSchemaKey
        {
            get
            {
                return ConfigurationManager.AppSettings["BetTypeSchemaKey"].ToString();
            }
        }

        public static string CachePrefix
        {
            get
            {
                return ConfigurationManager.AppSettings["Cache.Prefix"].ToString();
            }
        }

        public static int NumberTries
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["NumTries"]);
            }
        }

        public static string FromMail
        {
            get
            {
                return ConfigurationManager.AppSettings["FromMail"].ToString();
            }
        }

        public static string CouchbaseServer
        {
            get
            {
                return ConfigurationManager.AppSettings["CouchbaseServer"].ToString();
            }
        }

        public static string BucketOddsFeed
        {
            get
            {
                return ConfigurationManager.AppSettings["Bucket.OddsFeed"].ToString();
            }
        }

        public static string BucketOutrights
        {
            get
            {
                return ConfigurationManager.AppSettings["Bucket.Outrights"].ToString();
            }
        }

        public static string BucketLiveMatches
        {
            get
            {
                return ConfigurationManager.AppSettings["Bucket.LiveMatches"].ToString();
            }
        }

        public static string BucketDeadMatches
        {
            get
            {
                return ConfigurationManager.AppSettings["Bucket.DeadMatches"].ToString();
            }
        }
    }
}