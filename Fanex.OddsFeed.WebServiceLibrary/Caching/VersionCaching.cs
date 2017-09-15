namespace Fanex.OddsFeed.WebServiceLibrary.Caching
{
    using Common.Caching;
    using Fanex.OddsFeed.Common.Configurations;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.WebServiceLibrary.Models;
    using System;

    public class VersionCaching : IVersionCaching
    {
        private static readonly string _oddsFeedBucket = AppSettings.BucketOddsFeed;
        private static readonly string _currentVersionKey = CacheKeys.CurrentKey(AppSettings.CachePrefix);
        private static readonly string _isFullChainKey = CacheKeys.FullChainKey(AppSettings.CachePrefix);

        public CacheVersion GetRightVersion(OddsFeedsModel model)
        {
            var currentVersion = GetCurrentVersion();

            var isFullChain = IsFullChain();

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

        private static int GetCurrentVersion()
        {
            var currentVersion = 0;
            using (var bucket = ClusterRepository.GetBucket(_oddsFeedBucket))
            {
                currentVersion = bucket.Get<int>(_currentVersionKey).Value;
            }

            return currentVersion < 0 ? 0 : currentVersion;
        }

        private static bool IsFullChain()
        {
            using (var bucket = ClusterRepository.GetBucket(_oddsFeedBucket))
            {
                return bucket.Get<bool>(_isFullChainKey).Value;
            }
        }
    }
}