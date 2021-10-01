namespace Fanex.OddsFeed.WebService
{
    using System;
    using System.Diagnostics;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Common.Configurations;
    using Fanex.Caching;
    using Fanex.Caching.Redis;
    using Fanex.Data.Configuration;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions.Web;
    using Fanex.Logging.Sentry.RavenClient;
    using SharpRaven;

    public class MvcApplication : HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var _feedUrlPath = AppSettings.FeedUrlPath;
            if (!string.IsNullOrEmpty(_feedUrlPath))
            {
                _feedUrlPath += "/";
            }

            RegisterRoutesBetRadar(routes, _feedUrlPath);

            RegisterRoutesBedDaq(routes, _feedUrlPath);

            RegisterRoutesOddsFeed(routes, _feedUrlPath);

            RegisterRoutesSportsBetTypesLeaguesFeed(routes, _feedUrlPath);

            RegisterRoutesEvent(routes, _feedUrlPath);

            RegisterRoutesFeeds(routes, _feedUrlPath);

            routes.MapRoute(
                "OddsFeedWindowService",
                _feedUrlPath + "{username}/analytics/cache",
                new { controller = "Analytics", action = "Cache" });

            routes.MapRoute(
                "Default", // Route name
                _feedUrlPath + "{controller}/{action}/{id}", // URL with parameters
               new { controller = "Home", action = "Index", id = UrlParameter.Optional }); // Parameter defaults
        }

        protected void Application_Start()
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
            AreaRegistration.RegisterAllAreas();

            CacheServiceManager.Use(() => new RedisCache(new RedisCacheOptions
            {
                ConnectionString = AppSettings.RedisConnectionString,
                ApplicationName = AppSettings.RedisApplicationName
            }));

            GlobalFilters.Filters.Add(new HandleErrorAttribute());
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();

            ExceptionHandler.Handle(ex, TraceEventType.Error);
        }

        #region Private register routes methods

        private static void RegisterRoutesEvent(RouteCollection routes, string _feedUrlPath)
        {
            routes.MapRoute(
               "EventService",
               _feedUrlPath + "{username}/event.xml",
               new { controller = "Event", action = "Index" });
        }

        private static void RegisterRoutesFeeds(RouteCollection routes, string _feedUrlPath)
        {
            routes.MapRoute(
              "FeedsCsvService",
              _feedUrlPath + "{username}/{feedname}.csv",
              new { controller = "Feeds", action = "GetCsv" });

            routes.MapRoute(
              "FeedsXmlService",
              _feedUrlPath + "{username}/{feedname}.xml",
              new { controller = "Feeds", action = "Index" });

            routes.MapRoute(
                 "FeedsJsonService",
                 _feedUrlPath + "{username}/{feedname}.json",
                 new { controller = "Feeds", action = "GetJson" });
        }

        private static void RegisterRoutesOddsFeed(RouteCollection routes, string _feedUrlPath)
        {
            routes.MapRoute(
                "OddsFeedService",
                _feedUrlPath + "{username}/odds.xml",
                new { controller = "OddsFeed", action = "Index" });

            routes.MapRoute(
                "OddsFeedHelp",
                _feedUrlPath + "{username}/help.htm",
                new { controller = "OddsFeed", action = "Help" });

            routes.MapRoute(
              "OddsFeedOldHelp",
              _feedUrlPath + "{username}/oldhelp.htm",
              new { controller = "OddsFeed", action = "OldHelp" });
        }

        private static void RegisterRoutesBedDaq(RouteCollection routes, string _feedUrlPath)
        {
            routes.MapRoute(
                "CommonOddsFeedService", // Route name
                _feedUrlPath + "{username}/odds.txt", // URL with parameters
               new { controller = "BetDaq", action = "GetOdds" }); // Parameter defaults

            routes.MapRoute(
                "CommonScheduleService", // Route name
                _feedUrlPath + "{username}/sch.txt", // URL with parameters
               new { controller = "BetDaq", action = "GetSchedule" }); // Parameter defaults

            routes.MapRoute(
                "CommonInsertBetdaqService",
                _feedUrlPath + "{username}/InsertBet.cmd",
                new { controller = "InsertBetdaq", action = "Index" });

            routes.MapRoute(
                "BetDaqOddsFeedService", // Route name
                _feedUrlPath + "{username}/en-lang/odds.txt", // URL with parameters
               new { controller = "BetDaq", action = "GetOdds" }); // Parameter defaults

            routes.MapRoute(
                "BetDaqScheduleService", // Route name
                _feedUrlPath + "{username}/en-lang/sch.txt", // URL with parameters
               new { controller = "BetDaq", action = "GetSchedule" }); // Parameter defaults

            routes.MapRoute(
                "BetDaqInsertBetdaqService",
                _feedUrlPath + "{username}/en-lang/InsertBet.cmd",
                new { controller = "InsertBetdaq", action = "Index" });
        }

        private static void RegisterRoutesBetRadar(RouteCollection routes, string _feedUrlPath)
        {
            routes.MapRoute(
                "BetRadarOddsFeedService", // Route name
                _feedUrlPath + "{username}/oddsfeed.xml", // URL with parameters
               new { controller = "BetRadar", action = "Index" }); // Parameter defaults
        }

        private static void RegisterRoutesSportsBetTypesLeaguesFeed(RouteCollection routes, string _feedUrlPath)
        {
            routes.MapRoute(
                "SportService",
                _feedUrlPath + "{username}/sports.xml",
                new { controller = "Sport", action = "Index" });
            routes.MapRoute(
                "BetTypeService",
                _feedUrlPath + "{username}/bettypes.xml",
                new { controller = "BetType", action = "Index" });
            routes.MapRoute(
                "LeagueService",
                _feedUrlPath + "{username}/leagues.xml",
                new { controller = "League", action = "Index" });
        }

        #endregion Private register routes methods
    }
}