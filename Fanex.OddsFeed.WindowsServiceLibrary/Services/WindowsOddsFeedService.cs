namespace Fanex.OddsFeed.WindowsServiceLibrary.Services
{
    using Common;
    using Common.Configurations;
    using Common.DTO;
    using Common.Helpers;
    using Common.Schemas;
    using Couchbase;
    using Mapping;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public class WindowsOddsFeedService : IWindowsOddsFeedService
    {
        private static readonly string _cachePrefix = AppSettings.CachePrefix;
        private static readonly string _oddsFeedBucket = AppSettings.BucketOddsFeed;
        private static readonly string _liveMatchBucket = AppSettings.BucketLiveMatches;
        private static readonly string _deadMatchBucket = AppSettings.BucketDeadMatches;
        private static readonly string _outRightBucket = AppSettings.BucketOutrights;
        private static readonly List<Uri> _couchBaseServer = new List<Uri> { new Uri(AppSettings.CouchbaseServer) };
        private static readonly string _currentVersionKey = CacheKeys.CurrentKey(AppSettings.CachePrefix);
        private static int _totalVersion = AppSettings.TotalVersion;

        public void Reset()
        {
            Store(_oddsFeedBucket, AppSettings.BetTypeSchemaKey, new List<BetTypeSchema>());
            Store(_oddsFeedBucket, CacheKeys.CurrentKey(_cachePrefix), -1);
            Store(_oddsFeedBucket, CacheKeys.FullChainKey(_cachePrefix), false);
        }

        public bool Update()
        {
            var result = false;

            var betTypeSchemas = GetBetTypeSchemas();
            var liveMatches = GetLiveMatches(betTypeSchemas);
            var deadBallMatches = GetDeadballMatches(betTypeSchemas);
            var outrights = GetOutrights();

            var currentVersion = GetCurrentVersion() + 1;
            if (currentVersion == _totalVersion)
            {
                currentVersion = 0;
            }
            var versionKey = currentVersion.ToString();

            result = Store(_oddsFeedBucket, CacheKeys.CurrentKey(_cachePrefix), currentVersion);

            result &= Store(_liveMatchBucket, versionKey, liveMatches);
            result &= Store(_deadMatchBucket, versionKey, deadBallMatches);
            result &= Store(_outRightBucket, versionKey, outrights);

            if (result && currentVersion == _totalVersion - 1)
            {
                result &= Store(_oddsFeedBucket, CacheKeys.FullChainKey(_cachePrefix), true);
            }

            return result;
        }

        private static IList<Match> GetLiveMatches(IList<BetTypeSchema> betTypeSchemas)
        {
            using (var liveOdds = DbAccess.GetLiveOddsData())
            {
                var matchMapping = new MatchMapping();
                return matchMapping.GetMatches(liveOdds.Tables[0], betTypeSchemas, GetOddsFlexGroups(liveOdds.Tables[1]), true);
            }
        }

        private static IList<Match> GetDeadballMatches(IList<BetTypeSchema> betTypeSchemas)
        {
            using (var deadballOdds = DbAccess.GetDeadballOddsData())
            {
                var matchMapping = new MatchMapping();
                return matchMapping.GetMatches(deadballOdds.Tables[0], betTypeSchemas, GetOddsFlexGroups(deadballOdds.Tables[1]), false);
            }
        }

        private static IList<Outright> GetOutrights()
        {
            using (var outrightOdds = DbAccess.GetOutrightOddsData())
            {
                var outrightMapping = new OutrightMapping();
                return outrightMapping.GetOutrights(outrightOdds);
            }
        }

        private static IEnumerable<OddsFlexGroup> GetOddsFlexGroups(DataTable table)
        {
            var oddsFlexGroups = new List<OddsFlexGroup>();

            foreach (DataRow row in table.Rows)
            {
                var oddsId = Convert.ToInt32(row["oddsid"]);
                var oddsFlex = new OddsFlex
                {
                    BetChoice = row["betChoice"].ToString(),
                    BetType = Convert.ToInt32(row["bettype"]),
                    OddId = oddsId,
                    Odds = Convert.ToDecimal(row["odds"]),
                    OddsInfo = row["OddsInfo"].ToString()
                };
                var oddsFlexGroup = oddsFlexGroups.FirstOrDefault(odds => odds.OddsId == oddsId);

                if (oddsFlexGroup == null)
                {
                    oddsFlexGroups.Add(new OddsFlexGroup
                    {
                        OddsId = oddsId,
                        OddsFlexs = new List<OddsFlex> { oddsFlex }
                    });
                }
                else
                {
                    oddsFlexGroup.OddsFlexs.Add(oddsFlex);
                }
            }

            return oddsFlexGroups;
        }

        private static IList<BetTypeSchema> GetBetTypeSchemas()
        {
            var bucket = ClusterHelper.GetBucket(_oddsFeedBucket);
            var document = bucket.Get<IList<BetTypeSchema>>(AppSettings.BetTypeSchemaKey);

            if (document == null || document.Value == null || !document.Value.Any())
            {
                var newValue = JsonHelper.GetBetTypeSchemas<BetTypeSchema>(AppSettings.BetTypeSchemaPath);

                if (newValue != null)
                {
                    bucket.Upsert(AppSettings.BetTypeSchemaKey, newValue);

                    return newValue;
                }

                return new List<BetTypeSchema>();
            }

            return document.Value;
        }

        private static bool Store<T>(string bucketName, string key, T value)
        {
            var bucket = ClusterHelper.GetBucket(bucketName);

            return bucket.Upsert(key, value).Success;
        }

        private static int GetCurrentVersion()
        {
            var bucket = ClusterHelper.GetBucket(_oddsFeedBucket);

            return bucket.Get<int>(_currentVersionKey).Value;
        }
    }
}