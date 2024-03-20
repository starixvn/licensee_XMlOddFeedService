namespace OddsFeed.WebService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using OddsFeed.Models.Matches;
    using OddsFeed.Models.OddsData;
    using OddsFeed.Services;
    using OddsFeed.WebService.Core.Builders;
    using OddsFeed.WebService.Criteria;

    public class ResultFeedService : IResultFeedService
    {
        private readonly IMatchBuilder matchBuilder;
        private readonly IFeedCachingOperator feedCachingOperator;
        private readonly IResultFeedDataService resultFeedDataService;

        public ResultFeedService(
            IMatchBuilder matchBuilder,
            IFeedCachingOperator feedCachingOperator,
            IResultFeedDataService resultFeedDataService)
        {
            this.matchBuilder = matchBuilder;
            this.feedCachingOperator = feedCachingOperator;
            this.resultFeedDataService = resultFeedDataService;
        }

        public async Task<string> BuildXmlDocumentAsync(OddsFeedsModel model)
        {
            IList<Match> matches;
            if (model.Date.HasValue)
            {
                var cacheKey = $"XMLOddsFeed_MatchResultDetails_{model.Date.Value:ddMMyyyy}";
                matches = ConfiguredCacheService.Get<IList<Match>>(cacheKey);
                if (matches == null)
                {
                    const byte TimeExpireCache = 10;
                    matches = await GetMatchResultDetailAsync(model.Date.Value);
                    ConfiguredCacheService.Set(cacheKey, matches, new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(TimeExpireCache)));
                }
            }
            else
            {
                matches = await feedCachingOperator.GetMatchResultDetailAsync(model.UserProfile.ResultDelayTimes);
            }

            var xmlnode = new StringBuilder("<resultsfeed>");
            AppendMatches(xmlnode, matches, model);
            xmlnode.Append("</resultsfeed>");

            return xmlnode.ToString();
        }

        private async Task<IList<Match>> GetMatchResultDetailAsync(DateTime eventDate)
        {
            var criteria = new GetMatchResultDetailCriteria
            {
                EventDate = eventDate
            };

            return await resultFeedDataService.GetMatchResultDetailsAsync(criteria);
        }

        private void AppendMatches(
            StringBuilder xmlnode,
            IList<Match> matches,
            OddsFeedsModel model)
        {
            xmlnode.AppendFormat("<matches date=\"{0}\">", model.Date.HasValue ? model.Date.Value.Date : DateTimeOffset.Now.Date);
            matchBuilder.AppendMatchResultNodes(xmlnode, matches, model);
            xmlnode.Append("</matches>");
        }
    }
}