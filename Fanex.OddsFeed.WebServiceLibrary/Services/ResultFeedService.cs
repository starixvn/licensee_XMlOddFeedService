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
            xmlnode.AppendFormat("<matches date=\"{0}\">", model.Date.HasValue ? model.Date.Value.Date : DateTime.Today);
            matchBuilder.AppendMatchResultNodes(xmlnode, matches, model);
            xmlnode.Append("</matches>");
        }
    }
}