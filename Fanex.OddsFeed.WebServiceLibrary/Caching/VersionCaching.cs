namespace Fanex.OddsFeed.WebServiceLibrary.Caching
{
    using System;
    using Couchbase.Core;
    using Fanex.OddsFeed.Common.Configurations;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.WebServiceLibrary.Models;

    public class VersionCaching : IVersionCaching
    {
        private static readonly string _oddsFeedBucket = AppSettings.BucketOddsFeed;
        private static readonly string _currentVersionKey = CacheKeys.CurrentKey(AppSettings.CachePrefix);
        private static readonly string _isFullChainKey = CacheKeys.FullChainKey(AppSettings.CachePrefix);

        public CacheVersion GetRightVersion(ICluster cluster, OddsFeedsModel model)
        {
            var currentVersion = GetCurrentVersion(cluster);

            var isFullChain = IsFullChain(cluster);

            var delayTimeDeadball = 0;
            var delayTimeLive = 0;
            var delayTimeOutright = 0;
            if (model.UserProfile.DelayTimes.Count >= 3)
            {
                delayTimeDeadball = model.UserProfile.DelayTimes[0];
                delayTimeLive = model.UserProfile.DelayTimes[1];
                delayTimeOutright = model.UserProfile.DelayTimes[2];
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

        private static int GetCurrentVersion(ICluster cluster)
        {
            var currentVersion = 0;
            using (var bucket = cluster.OpenBucket(_oddsFeedBucket))
            {
                currentVersion = bucket.Get<int>(_currentVersionKey).Value;
            }

            return currentVersion < 0 ? 0 : currentVersion;
        }

        private static bool IsFullChain(ICluster cluster)
        {
            using (var bucket = cluster.OpenBucket(_oddsFeedBucket))
            {
                return bucket.Get<bool>(_isFullChainKey).Value;
            }
        }
    }
}