namespace Fanex.OddsFeed.WindowsServiceLibrary.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.Logging;
    using Fanex.OddsFeed.Common;
    using Fanex.OddsFeed.Common.Configurations;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.Common.Helpers;
    using Fanex.OddsFeed.Common.Repository;
    using Fanex.OddsFeed.Common.Schemas;

    public class WindowsOddsFeedService : IWindowsOddsFeedService
    {
        private static readonly string cachePrefix = AppSettings.CachePrefix;
        private static readonly string oddsFeedBucket = AppSettings.BucketOddsFeed;
        private static readonly string liveMatchBucket = AppSettings.BucketLiveMatches;
        private static readonly string deadMatchBucket = AppSettings.BucketDeadMatches;
        private static readonly string outRightBucket = AppSettings.BucketOutrights;
        private static readonly string currentVersionKey = CacheKeys.CurrentKey(AppSettings.CachePrefix);
        private static readonly int totalVersion = AppSettings.TotalVersion;

        private readonly IOddsDataService oddsDataService;

        public WindowsOddsFeedService()
        {
            oddsDataService = new OddsDataService();
        }

        public void Reset()
        {
            Store(oddsFeedBucket, AppSettings.BetTypeSchemaKey, new List<BetTypeSchema>());
            Store(oddsFeedBucket, CacheKeys.CurrentKey(cachePrefix), -1);
            Store(oddsFeedBucket, CacheKeys.FullChainKey(cachePrefix), false);
        }

        public bool Update()
        {
            bool result;
            var betTypeSchemas = GetBetTypeSchemas();
            var liveMatches = oddsDataService.GetLiveMatches(betTypeSchemas);
            var deadBallMatches = oddsDataService.GetDeadballMatches(betTypeSchemas);

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

            var outrights = oddsDataService.GetOutrights();

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

        private static string GetExceptionMessage(Exception exception)
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

        private static IList<BetTypeSchema> GetBetTypeSchemas()
        {
            var document = RedisRepository.ReadData<IList<BetTypeSchema>>(string.Format(Constants.RedisKeyFormat, oddsFeedBucket, AppSettings.BetTypeSchemaKey));

            if (document == null || document.Count == 0)
            {
                var newValue = JsonHelper.GetBetTypeSchemas<BetTypeSchema>(AppSettings.BetTypeSchemaPath);

                if (newValue != null)
                {
                    RedisRepository.StoreData<IList<BetTypeSchema>>(string.Format(Constants.RedisKeyFormat, oddsFeedBucket, AppSettings.BetTypeSchemaKey), newValue);
                    return newValue;
                }

                return new List<BetTypeSchema>();
            }

            return document;
        }

        private static bool Store<T>(string bucketName, string key, T value)
        {
            try
            {
                return RedisRepository.StoreData<T>(string.Format(Constants.RedisKeyFormat, bucketName, key), value);
            }
            catch (Exception ex)
            {
                var message = GetExceptionMessage(ex);
                LogService.Error(message);
                return false;
            }
        }

        private static int GetCurrentVersion()
        {
            return RedisRepository.ReadData<int>(string.Format(Constants.RedisKeyFormat, oddsFeedBucket, currentVersionKey));
        }
    }
}