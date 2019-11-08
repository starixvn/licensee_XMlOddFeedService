namespace Fanex.OddsFeed.WebServiceLibrary.Caching
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Schemas;
    using Couchbase;
    using Fanex.OddsFeed.Common.Configurations;
    using Fanex.OddsFeed.Common.DTO;
    using Models;

    public class OddsFeedCachingOperator : IOddsFeedCachingOperator
    {
        public OddsData GetOddsData(OddsFeedsModel model, bool isGetOutright, bool isGetLiveMatch, bool isGetDeadballMatch)
        {
            var versionCaching = new VersionCaching();
            var cacheVersion = versionCaching.GetRightVersion(model);

            var outrights = isGetOutright
                ? Get<Outright>(AppSettings.BucketOutrights, cacheVersion.CurrentVersionOutright.ToString()) :
                new List<Outright>();

            var liveMatches = isGetLiveMatch
                ? Get<Match>(AppSettings.BucketLiveMatches, cacheVersion.CurrentVersionLiveMatch.ToString())
                : new List<Match>();

            var deadballs1 = new List<Match>();
            var deadballs2 = new List<Match>();

            if (isGetDeadballMatch)
            {
                deadballs1 = Get<Match>(AppSettings.BucketDeadMatches, cacheVersion.CurrentVersionDeadballMatch.ToString() + "_1").ToList();
                deadballs2 = Get<Match>(AppSettings.BucketDeadMatches, cacheVersion.CurrentVersionDeadballMatch.ToString() + "_2").ToList();
            }

            var deadballMatches = deadballs1.Union(deadballs2).ToList();

            return new OddsData
            {
                Outrights = outrights,
                LiveMatches = liveMatches,
                DeadballMatches = deadballMatches
            };
        }

        public IList<BetTypeSchema> GetBetTypes()
        {
            return Get<BetTypeSchema>(AppSettings.BucketOddsFeed, AppSettings.BetTypeSchemaKey);
        }

        private static IList<T> Get<T>(string bucketName, string key)
        {
            var bucket = ClusterHelper.GetBucket(bucketName);
            var result = bucket.Get<IList<T>>(key);

            return result.Success ? result.Value : new List<T>();
        }
    }
}