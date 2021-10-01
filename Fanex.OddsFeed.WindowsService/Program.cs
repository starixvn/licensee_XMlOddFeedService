namespace Fanex.OddsFeed.WindowsService
{
    using System.ServiceProcess;
    using Data.Configuration;
    using Fanex.Caching;
    using Fanex.Caching.Redis;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions.Web;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.OddsFeed.Common.Configurations;
    using SharpRaven;

    public static class Program
    {
        public static void Main()
        {
            LogManager
                   .SetDefaultLogCategory(AppSettings.SentryDNS)
                   .Use(new SentryLogging(new SentryEngineOptions
                   {
                       Dsn = new Dsn(AppSettings.SentryDNS)
                   }));

            ExceptionHandler
                .UseConfig(new LoggingConfiguration())
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            DbSettingProviderManager.Current.Start();
            var servicesToRun = new ServiceBase[]
            {
                new OddsFeedService()
            };

            CacheServiceManager.Use(() => new RedisCache(new RedisCacheOptions
            {
                ConnectionString = AppSettings.RedisConnectionString,
                ApplicationName = AppSettings.RedisApplicationName
            }));

            ServiceBase.Run(servicesToRun);
        }
    }
}