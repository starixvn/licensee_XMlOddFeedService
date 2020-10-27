namespace Fanex.OddsFeed.WebServiceLibrary.Caching
{
    using System;
    using Couchbase;
    using Fanex.OddsFeed.Common.Configurations;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.WebServiceLibrary.Models;

    public class VersionCaching : IVersionCaching
    {
        private static readonly string _oddsFeedBucket = AppSettings.BucketOddsFeed;
        private static readonly string _currentVersionKey = CacheKeys.CurrentKey(AppSettings.CachePrefix);
        private static readonly string _isFullChainKey = CacheKeys.FullChainKey(AppSettings.CachePrefix);

        public CacheVersion GetRightVersion(OddsFeedsModel model)
        {
            const int maxDelayTimes = 3;
            var currentVersion = GetCurrentVersion();
            var isFullChain = IsFullChain();
            var delayTimeDeadball = 0;
            var delayTimeLive = 0;
            var delayTimeOutright = 0;
            if (model.UserProfile.DelayTimes.Count >= maxDelayTimes)
            {
                var index = 0;
                delayTimeDeadball = model.UserProfile.DelayTimes[index++];
                delayTimeLive = model.UserProfile.DelayTimes[index++];
                delayTimeOutright = model.UserProfile.DelayTimes[index];
            }

            return new CacheVersion
            {
                CurrentVersionOutright = GetRightVersion(delayTimeOutright, currentVersion, isFullChain),
                CurrentVersionLiveMatch = GetRightVersion(delayTimeLive, currentVersion, isFullChain),
                CurrentVersionDeadballMatch = GetRightVersion(delayTimeDeadball, currentVersion, isFullChain)
            };
        }

        private static int GetRightVersion(int delayTime, int currentVersion, bool isFullChain)
        {
            var rightVersion = currentVersion - (delayTime / AppSettings.Interval);

            if (isFullChain)
            {
                return rightVersion > -1 ? rightVersion : AppSettings.TotalVersion - Math.Abs(rightVersion);
            }

            return rightVersion > -1 ? rightVersion : -1;
        }

        private static int GetCurrentVersion()
        {
            var bucket = ClusterHelper.GetBucket(_oddsFeedBucket);
            var currentVersion = bucket.Get<int>(_currentVersionKey).Value;

            return currentVersion < 0 ? 0 : currentVersion;
        }

        private static bool IsFullChain()
        {
            var bucket = ClusterHelper.GetBucket(_oddsFeedBucket);
            return bucket.Get<bool>(_isFullChainKey).Value;
        }
    }
}