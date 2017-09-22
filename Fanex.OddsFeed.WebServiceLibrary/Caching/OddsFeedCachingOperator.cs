namespace Fanex.OddsFeed.WebServiceLibrary.Caching
{
    using Common.Schemas;
    using Couchbase;
    using Couchbase.Configuration.Client;
    using Fanex.OddsFeed.Common.Configurations;
    using Fanex.OddsFeed.Common.DTO;
    using Models;
    using System;
    using System.Collections.Generic;

    public class OddsFeedCachingOperator : IOddsFeedCachingOperator
    {
        private static readonly ClientConfiguration _couchBaseClientConfig = new ClientConfiguration
        {
            Servers = new List<Uri> { new Uri(AppSettings.CouchbaseServer) }
        };

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

            var deadballMatches = isGetDeadballMatch
                ? Get<Match>(AppSettings.BucketDeadMatches, cacheVersion.CurrentVersionDeadballMatch.ToString())
                : new List<Match>();

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