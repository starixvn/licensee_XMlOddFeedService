namespace Fanex.OddsFeed.Web.Services.Services
{
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.OddsFeed.Models.Matches;
    using Fanex.OddsFeed.Models.OddsData;
    using Fanex.OddsFeed.Repository;
    using Fanex.OddsFeed.Web.Services.Builders;
    using Fanex.OddsFeed.Web.Services.Criteria;
    using Fanex.OddsFeed.Web.Services.Services.Interfaces;

    public class ResultFeedService : IResultFeedService
    {
        private readonly IFeedCachingOperator feedCachingOperator;
        private readonly IDbAccessRepository dbAccessRepository;

        public ResultFeedService(IFeedCachingOperator feedCachingOperator, IDbAccessRepository dbAccessRepository)
        {
            this.feedCachingOperator = feedCachingOperator;
            this.dbAccessRepository = dbAccessRepository;
        }

        public async Task<string> BuildXmlDocument(OddsFeedsModel model)
        {
            IList<Match> matches;
            if (model.Date.HasValue)
            {
                var cacheKey = $"XMLOddsFeed_MatchResultDetails_{model.Date.Value.ToString("ddMMyyyy")}";
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
                matches = await feedCachingOperator.GetMatchResultDetails(model.UserProfile.ResultDelayTimes);
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

            return await dbAccessRepository.GetMatchResultDetails(criteria);
        }

        private void AppendMatches(
            StringBuilder xmlnode,
            IList<Match> matches,
            OddsFeedsModel model)
        {
            xmlnode.AppendFormat("<matches date=\"{0}\">", model.Date.HasValue ? model.Date.Value.Date : DateTime.Today);
            MatchBuilder.AppendMatchResultNodes(xmlnode, matches, model);
            xmlnode.Append("</matches>");
        }
    }
}