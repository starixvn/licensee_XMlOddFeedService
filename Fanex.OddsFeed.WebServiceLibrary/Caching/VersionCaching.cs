namespace Fanex.OddsFeed.WebServiceLibrary.Caching
{
    using System;
    using Fanex.OddsFeed.Common.Configurations;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.Common.Repository;

    public class VersionCaching : IVersionCaching
    {
        private static readonly string oddsFeedBucket = AppSettings.BucketOddsFeed;
        private static readonly string resultFeedBucket = AppSettings.ResultFeed_Bucket;
        private static readonly string currentVersionKey = CacheKeys.CurrentKey(AppSettings.CachePrefix);
        private static readonly string isFullChainKey = CacheKeys.FullChainKey(AppSettings.CachePrefix);

        public CacheVersion GetRightVersionOddsFeed(UserProfile profile)
        {
            const int maxDelayTimes = 3;
            var currentVersion = GetCurrentVersion(oddsFeedBucket);
            var isFullChain = IsFullChain(oddsFeedBucket);
            var delayTimeDeadball = 0;
            var delayTimeLive = 0;
            var delayTimeOutright = 0;
            if (profile.OddsDelayTimes.Count >= maxDelayTimes)
            {
                var index = 0;
                delayTimeDeadball = profile.OddsDelayTimes[index++];
                delayTimeLive = profile.OddsDelayTimes[index++];
                delayTimeOutright = profile.OddsDelayTimes[index];
            }

            return new CacheVersion
            {
                CurrentVersionOutright = GetRightVersionOddsFeed(delayTimeOutright, currentVersion, isFullChain),
                CurrentVersionLiveMatch = GetRightVersionOddsFeed(delayTimeLive, currentVersion, isFullChain),
                CurrentVersionDeadballMatch = GetRightVersionOddsFeed(delayTimeDeadball, currentVersion, isFullChain)
            };
        }

        public int GetRightVersionResultFeed(int delayTimes)
        {
            var currentVersion = GetCurrentVersion(resultFeedBucket);
            var isFullChain = IsFullChain(resultFeedBucket);
            var rightVersion = currentVersion - (delayTimes / AppSettings.ResultFeed_Interval);
            if (isFullChain)
            {
                return rightVersion > -1 ? rightVersion : AppSettings.ResultFeed_TotalVersion - Math.Abs(rightVersion);
            }

            return rightVersion > -1 ? rightVersion : -1;
        }

        private static int GetRightVersionOddsFeed(int delayTime, int currentVersion, bool isFullChain)
        {
            var rightVersion = currentVersion - (delayTime / AppSettings.Interval);
            if (isFullChain)
            {
                return rightVersion > -1 ? rightVersion : AppSettings.TotalVersion - Math.Abs(rightVersion);
            }

            return rightVersion > -1 ? rightVersion : -1;
        }

        private static int GetCurrentVersion(string bucket)
        {
            var currentVersion = RedisRepository.ReadData<int>(string.Format("{0}:{1}", bucket, currentVersionKey));
            return currentVersion < 0 ? 0 : currentVersion;
        }

        private static bool IsFullChain(string bucket)
        {
            return RedisRepository.ReadData<bool>(string.Format("{0}:{1}", bucket, isFullChainKey));
        }
    }
}