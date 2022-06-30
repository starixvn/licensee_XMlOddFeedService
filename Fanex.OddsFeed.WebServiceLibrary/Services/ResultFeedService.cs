namespace Fanex.OddsFeed.WebServiceLibrary.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Caching;
    using Fanex.OddsFeed.Common;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.WebServiceLibrary.Builders;
    using Fanex.OddsFeed.WebServiceLibrary.Caching;
    using Fanex.OddsFeed.WebServiceLibrary.Models;

    public class ResultFeedService : IResultFeedService
    {
        private readonly IMatchBuilder matchBuilder;
        private readonly IFeedCachingOperator feedCachingOperator;

        public ResultFeedService()
        {
            matchBuilder = new MatchBuilder();
            feedCachingOperator = new FeedCachingOperator();
        }

        public string BuildXmlDocument(OddsFeedsModel model)
        {
            IList<Match> matches;
            if (model.Date.HasValue)
            {
                var cacheKey = $"XMLOddsFeed_MatchResultDetails_{model.Date.Value.ToString("ddMMyyyy")}";
                matches = ConfiguredCacheService.Get<IList<Match>>(cacheKey);
                if(matches == null)
                {
                    const byte TimeExpireCache = 10;
                    matches = DbAccess.GetMatchResultDetails(model.Date.Value);
                    ConfiguredCacheService.Set(cacheKey, matches, new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(TimeExpireCache)));
                }
            }
            else
            {
                matches = feedCachingOperator.GetMatchResultDetails(model.UserProfile.ResultDelayTimes);
            }

            var xmlnode = new StringBuilder("<resultsfeed>");
            AppendMatches(xmlnode, matches, model);
            xmlnode.Append("</resultsfeed>");

            return xmlnode.ToString();
        }

        private void AppendMatches(
            StringBuilder xmlnode,
            IList<Match> matches,
            OddsFeedsModel model)
        {
            AppendBeginMatches(xmlnode, GetMatchTimestamp(matches), GetLastUpdate());

            matchBuilder.AppendMatchNodes(xmlnode, matches, model);

            xmlnode.Append("</matches>");
        }

        private static int GetMatchTimestamp(IList<Match> matches)
        {
            return matches.Max(x => x.Timestamp);
        }

        private static DateTime GetLastUpdate()
        {
            const int addHour = 12;
            return DateTime.Now.AddHours(addHour);
        }

        private static void AppendBeginMatches(
            StringBuilder xmlnode,
            int matchesTimestamp,
            DateTime lastUpdate)
        {
            if (matchesTimestamp > 0)
            {
                xmlnode.AppendFormat("<matches lastupdate=\"{0}\" timestamp=\"{1}\">", lastUpdate, matchesTimestamp);
            }
            else
            {
                xmlnode.Append("<matches>");
            }
        }
    }
}