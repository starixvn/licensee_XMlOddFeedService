namespace Fanex.OddsFeed.WindowsServiceLibrary.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Common;
    using Common.Configurations;
    using Common.DTO;
    using Common.Helpers;
    using Common.Schemas;
    using Couchbase;
    using Fanex.Logging;
    using Mapping;
    using NLog;

    public class WindowsOddsFeedService : IWindowsOddsFeedService
    {
        private readonly NLog.Logger fileLogger = LogManager.GetCurrentClassLogger();
        private static readonly string cachePrefix = AppSettings.CachePrefix;
        private static readonly string oddsFeedBucket = AppSettings.BucketOddsFeed;
        private static readonly string liveMatchBucket = AppSettings.BucketLiveMatches;
        private static readonly string deadMatchBucket = AppSettings.BucketDeadMatches;
        private static readonly string outRightBucket = AppSettings.BucketOutrights;
        private static readonly string currentVersionKey = CacheKeys.CurrentKey(AppSettings.CachePrefix);
        private static readonly int totalVersion = AppSettings.TotalVersion;

        public void Reset()
        {
            Store(oddsFeedBucket, AppSettings.BetTypeSchemaKey, new List<BetTypeSchema>());
            Store(oddsFeedBucket, CacheKeys.CurrentKey(cachePrefix), -1);
            Store(oddsFeedBucket, CacheKeys.FullChainKey(cachePrefix), false);
        }

        public bool Update()
        {
            var result = false;

            var betTypeSchemas = GetBetTypeSchemas();
            var liveMatches = GetLiveMatches(betTypeSchemas);
            var deadBallMatches = GetDeadballMatches(betTypeSchemas);
            var deadBallLength = deadBallMatches.Count;
            var splitIndex = deadBallLength / 2;
            var deadBalls1 = new List<Match>();
            var deadBalls2 = new List<Match>();

            for (var i = 0; i < splitIndex; i++)
            {
                deadBalls1.Add(deadBallMatches[i]);
            }

            for (var j = splitIndex; j < deadBallLength; j++)
            {
                deadBalls2.Add(deadBallMatches[j]);
            }

            var deadball3 = deadBalls1.Union(deadBalls2).ToList();

            fileLogger.Trace(deadBallMatches.Count);
            fileLogger.Trace(deadBalls1.Count);
            fileLogger.Trace(deadBalls2.Count);
            fileLogger.Trace(deadball3.Count);

            var outrights = GetOutrights();

            var currentVersion = GetCurrentVersion() + 1;
            if (currentVersion == totalVersion)
            {
                currentVersion = 0;
            }
            var versionKey = currentVersion.ToString();

            result = Store(oddsFeedBucket, CacheKeys.CurrentKey(cachePrefix), currentVersion);

            result &= Store(liveMatchBucket, versionKey, liveMatches);
            result &= Store(deadMatchBucket, versionKey + "_1", deadBalls1);
            result &= Store(deadMatchBucket, versionKey + "_2", deadBalls2);
            result &= Store(outRightBucket, versionKey, outrights);

            if (result && currentVersion == totalVersion - 1)
            {
                result &= Store(oddsFeedBucket, CacheKeys.FullChainKey(cachePrefix), true);
            }

            return result;
        }

        private string GetExceptionMessage(Exception exception)
        {
            var messages = new[]
            {
                "ENVIRONMENT: " + AppSettings.Environment,
                "ERROR: " + exception.Message,
                "SOURCE: " + exception.Source,
                "TARGETSITE: " + exception.TargetSite,
                "STACKTRACE: " + exception.StackTrace
            };

            return string.Join(Environment.NewLine, messages);
        }

        private static IList<Match> GetLiveMatches(IList<BetTypeSchema> betTypeSchemas)
        {
            using (var liveOdds = DbAccess.GetLiveOddsData())
            {
                var matchMapping = new MatchMapping();
                return matchMapping.GetMatches(liveOdds.Tables[0], betTypeSchemas, GetOddsFlexGroups(liveOdds.Tables[1]), true);
            }
        }

        private IList<Match> GetDeadballMatches(IList<BetTypeSchema> betTypeSchemas)
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
            var bucket = ClusterHelper.GetBucket(oddsFeedBucket);
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

        private bool Store<T>(string bucketName, string key, T value)
        {
            var bucket = ClusterHelper.GetBucket(bucketName);
            var result = bucket.Upsert(key, value);

            if (!result.Success)
            {
                LogService.Error(result.Message);
                if (result.Exception != null)
                {
                    var exception = result.Exception;
                    var message = GetExceptionMessage(exception);
                    LogService.Error(message);
                }
            }

            return result.Success;
        }

        private static int GetCurrentVersion()
        {
            var bucket = ClusterHelper.GetBucket(oddsFeedBucket);

            return bucket.Get<int>(currentVersionKey).Value;
        }
    }
}