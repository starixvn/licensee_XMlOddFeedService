namespace Fanex.OddsFeed.Web.Extensions
{
    public static class RouteHelper
    {
        private const string betDaq = "BetDaq";
        private const string index = "Index";

        public static void RegisterXMLOddsfeedRoutes(this IApplicationBuilder app, string feedUrlPath)
        {
            app.UseMvc(routes =>
            {
                RegisterRoutesBetRadar(routes, feedUrlPath);

                RegisterRoutesBedDaq(routes, feedUrlPath);

                RegisterRoutesOddsFeed(routes, feedUrlPath);

                RegisterRoutesSportsBetTypesLeaguesFeed(routes, feedUrlPath);

                RegisterRoutesEvent(routes, feedUrlPath);

                RegisterRoutesFeeds(routes, feedUrlPath);

                RegisterRoutesCricketResultFeed(routes, feedUrlPath);
            });
        }

        private static void RegisterRoutesEvent(IRouteBuilder routes, string feedUrlPath)
        {
            routes.MapRoute(
               "EventService",
               feedUrlPath + "{username}/event.xml",
               new { controller = "Event", action = index });
        }

        private static void RegisterRoutesFeeds(IRouteBuilder routes, string feedUrlPath)
        {
            routes.MapRoute(
              "FeedsCsvService",
              feedUrlPath + "{username}/{feedname}.csv",
              new { controller = "Feeds", action = "GetCsv" });

            routes.MapRoute(
              "FeedsXmlService",
              feedUrlPath + "{username}/{feedname}.xml",
              new { controller = "Feeds", action = index });

            routes.MapRoute(
                 "FeedsJsonService",
                 feedUrlPath + "{username}/{feedname}.json",
                 new { controller = "Feeds", action = "GetJson" });
        }

        private static void RegisterRoutesOddsFeed(IRouteBuilder routes, string feedUrlPath)
        {
            routes.MapRoute(
                "OddsFeedService",
                feedUrlPath + "{username}/odds.xml",
                new { controller = "OddsFeed", action = index });

            routes.MapRoute(
                "OddsFeedHelp",
                feedUrlPath + "{username}/help.htm",
                new { controller = "OddsFeed", action = "Help" });

            routes.MapRoute(
              "OddsFeedOldHelp",
              feedUrlPath + "{username}/oldhelp.htm",
              new { controller = "OddsFeed", action = "OldHelp" });
        }

        private static void RegisterRoutesBedDaq(IRouteBuilder routes, string feedUrlPath)
        {
            routes.MapRoute(
                "CommonOddsFeedService", // Route name
                feedUrlPath + "{username}/odds.txt", // URL with parameters
               new { controller = betDaq, action = "GetOdds" }); // Parameter defaults

            routes.MapRoute(
                "CommonScheduleService", // Route name
                feedUrlPath + "{username}/sch.txt", // URL with parameters
               new { controller = betDaq, action = "GetSchedule" }); // Parameter defaults

            routes.MapRoute(
                "CommonInsertBetdaqService",
                feedUrlPath + "{username}/InsertBet.cmd",
                new { controller = "InsertBetdaq", action = index });

            routes.MapRoute(
                "BetDaqOddsFeedService", // Route name
                feedUrlPath + "{username}/en-lang/odds.txt", // URL with parameters
               new { controller = betDaq, action = "GetOdds" }); // Parameter defaults

            routes.MapRoute(
                "BetDaqScheduleService", // Route name
                feedUrlPath + "{username}/en-lang/sch.txt", // URL with parameters
               new { controller = betDaq, action = "GetSchedule" }); // Parameter defaults

            routes.MapRoute(
                "BetDaqInsertBetdaqService",
                feedUrlPath + "{username}/en-lang/InsertBet.cmd",
                new { controller = "InsertBetdaq", action = index });
        }

        private static void RegisterRoutesBetRadar(IRouteBuilder routes, string feedUrlPath)
        {
            routes.MapRoute(
                "BetRadarOddsFeedService", // Route name
                feedUrlPath + "{username}/oddsfeed.xml", // URL with parameters
               new { controller = "BetRadar", action = index }); // Parameter defaults
        }

        private static void RegisterRoutesSportsBetTypesLeaguesFeed(IRouteBuilder routes, string feedUrlPath)
        {
            routes.MapRoute(
                "SportService",
                feedUrlPath + "{username}/sports.xml",
                new { controller = "Sport", action = index });
            routes.MapRoute(
                "BetTypeService",
                feedUrlPath + "{username}/bettypes.xml",
                new { controller = "BetType", action = index });
            routes.MapRoute(
                "LeagueService",
                feedUrlPath + "{username}/leagues.xml",
                new { controller = "League", action = index });
        }

        private static void RegisterRoutesCricketResultFeed(IRouteBuilder routes, string feedUrlPath)
        {
            routes.MapRoute(
                "CricketResultFeedService",
                feedUrlPath + "{username}/cricket/result.xml",
                new { controller = "CricketResultFeed", action = index });
        }
    }
}