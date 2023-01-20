namespace Fanex.OddsFeed.Web.Services.Services
{
    using Fanex.OddsFeed.Core.Services;
    using Fanex.OddsFeed.Models;
    using Fanex.OddsFeed.Models.Matches;
    using Fanex.OddsFeed.Models.OddsData;
    using Fanex.OddsFeed.Models.Outright;
    using Fanex.OddsFeed.Web.Services.Models;
    using Fanex.OddsFeed.Web.Services.Services.Interfaces;

    public class FeedCachingOperator : IFeedCachingOperator
    {
        private readonly WebSettings webSettings;
        private readonly IVersionCachingService versionCachingService;
        private readonly IRedisCacheService redisCacheService;

        public FeedCachingOperator(WebSettings webSettings, IVersionCachingService versionCachingService, IRedisCacheService redisCacheService)
        {
            this.webSettings = webSettings;
            this.versionCachingService = versionCachingService;
            this.redisCacheService = redisCacheService;
        }

        public OddsData GetOddsData(OddsFeedsModel model, bool isGetOutright, bool isGetLiveMatch, bool isGetDeadballMatch)
        {
            var cacheVersion = versionCachingService.GetRightVersionOddsFeed(model.UserProfile);

            var outrights = isGetOutright
                ? Get<Outright>(webSettings.Bucket.Outrights, cacheVersion.CurrentVersionOutright.ToString()) :
                new List<Outright>();

            var liveMatches = isGetLiveMatch
                ? Get<Match>(webSettings.Bucket.LiveMatches, cacheVersion.CurrentVersionLiveMatch.ToString())
                : new List<Match>();

            var deadballs1 = new List<Match>();
            var deadballs2 = new List<Match>();

            if (isGetDeadballMatch)
            {
                deadballs1 = Get<Match>(webSettings.Bucket.DeadMatches, cacheVersion.CurrentVersionDeadballMatch.ToString() + "_1").ToList();
                deadballs2 = Get<Match>(webSettings.Bucket.DeadMatches, cacheVersion.CurrentVersionDeadballMatch.ToString() + "_2").ToList();
            }

            var deadballMatches = deadballs1.Union(deadballs2).ToList();

            return new OddsData
            {
                Outrights = outrights.OrderBy(t => t.EventId).ToList(),
                LiveMatches = liveMatches.OrderBy(t => t.MatchId).ToList(),
                DeadballMatches = deadballMatches.OrderBy(t => t.MatchId).ToList()
            };
        }

        public IList<BetTypeSchema> GetBetTypes()
        {
            return Get<BetTypeSchema>(webSettings.Bucket.OddsFeed, webSettings.BetTypeSchemaKey);
        }

        public IList<Match> GetMatchResultDetails(int delayTimes)
        {
            var cacheVersion = versionCachingService.GetRightVersionResultFeed(delayTimes);
            return Get<Match>(webSettings.ResultFeed.BucketData, cacheVersion.ToString());
        }

        private IList<T> Get<T>(string bucketName, string key)
        {
            var result = redisCacheService.Get<IList<T>>(string.Format("{0}:{1}", bucketName, key));
            return result ?? new List<T>();
        }
    }
}