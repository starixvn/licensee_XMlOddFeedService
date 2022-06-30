namespace Fanex.OddsFeed.WindowsServiceLibrary.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.Logging;
    using Fanex.OddsFeed.Common;
    using Fanex.OddsFeed.Common.Configurations;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.Common.Repository;

    public class ResultFeedService : IResultFeedService
    {
        private static readonly string cachePrefix = AppSettings.CachePrefix;
        private static readonly string resultFeedBucket = AppSettings.ResultFeed_Bucket;
        private static readonly string resultFeedBucketData = AppSettings.ResultFeed_BucketData;
        private static readonly string currentVersionKey = CacheKeys.CurrentKey(AppSettings.CachePrefix);
        private static readonly int totalVersion = AppSettings.ResultFeed_TotalVersion;

        public void Reset()
        {
            Store(resultFeedBucket, CacheKeys.CurrentKey(cachePrefix), -1);
            Store(resultFeedBucket, CacheKeys.FullChainKey(cachePrefix), false);
        }

        public bool Update()
        {
            bool result;

            var currentVersion = GetCurrentVersion() + 1;
            if (currentVersion == totalVersion)
            {
                currentVersion = 0;
            }
            var versionKey = currentVersion.ToString();
            IList<Match> matches = DbAccess.GetMatchResultDetails(DateTime.Today);

            result = Store(resultFeedBucket, CacheKeys.CurrentKey(cachePrefix), currentVersion);
            result &= Store(resultFeedBucketData, versionKey, matches);

            if (result && currentVersion == totalVersion - 1)
            {
                result &= Store(resultFeedBucket, CacheKeys.FullChainKey(cachePrefix), true);
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
            return RedisRepository.ReadData<int>(string.Format(Constants.RedisKeyFormat, resultFeedBucket, currentVersionKey));
        }
    }
}