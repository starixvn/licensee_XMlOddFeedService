namespace Fanex.OddsFeed.WebServiceLibrary.Caching
{
    using System;
    using System.Collections.Generic;
    using Common.Schemas;
    using Couchbase;
    using Couchbase.Configuration.Client;
    using Couchbase.Core;
    using Fanex.OddsFeed.Common.Configurations;
    using Fanex.OddsFeed.Common.DTO;
    using Models;

    public class OddsFeedCachingOperator : IOddsFeedCachingOperator
    {
        private static readonly ClientConfiguration _couchBaseClientConfig = new ClientConfiguration
        {
            Servers = new List<Uri> { new Uri(AppSettings.CouchbaseServer) }
        };

        public OddsData GetOddsData(OddsFeedsModel model, bool isGetOutright, bool isGetLiveMatch, bool isGetDeadballMatch)
        {
            using (var cluster = new Cluster(_couchBaseClientConfig))
            {
                var versionCaching = new VersionCaching();
                var cacheVersion = versionCaching.GetRightVersion(cluster, model);

                var outrights = isGetOutright
                    ? Get<Outright>(cluster, AppSettings.BucketOutrights, cacheVersion.CurrentVersionOutright.ToString()) :
                    new List<Outright>();

                var liveMatches = isGetLiveMatch
                    ? Get<Match>(cluster, AppSettings.BucketLiveMatches, cacheVersion.CurrentVersionLiveMatch.ToString())
                    : new List<Match>();

                var deadballMatches = isGetDeadballMatch
                    ? Get<Match>(cluster, AppSettings.BucketDeadMatches, cacheVersion.CurrentVersionDeadballMatch.ToString())
                    : new List<Match>();

                return new OddsData
                {
                    Outrights = outrights,
                    LiveMatches = liveMatches,
                    DeadballMatches = deadballMatches
                };
            }
        }

        public IList<BetTypeSchema> GetBetTypes()
        {
            using (var cluster = new Cluster(_couchBaseClientConfig))
            {
                return Get<BetTypeSchema>(cluster, AppSettings.BucketOddsFeed, AppSettings.BetTypeSchemaKey);
            }
        }

        private static IList<T> Get<T>(ICluster cluster, string bucketName, string key)
        {
            using (var bucket = cluster.OpenBucket(bucketName))
            {
                var result = bucket.Get<IList<T>>(key);

                return result.Success ? result.Value : new List<T>();
            }
        }
    }
}