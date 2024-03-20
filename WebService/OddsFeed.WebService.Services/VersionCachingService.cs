namespace OddsFeed.WebService.Services
{
    using System;
    using OddsFeed.Core.Extensions;
    using OddsFeed.Core.Services;
    using OddsFeed.Models.DTO;
    using OddsFeed.WebService.Models;
    using OddsFeed.WebService.Models.Cache;

    public class VersionCachingService : IVersionCachingService
    {
        private readonly WebSettings appSettings;
        private readonly IRedisCacheService redisCacheService;

        public VersionCachingService(WebSettings appSettings, IRedisCacheService redisCacheService)
        {
            this.appSettings = appSettings;
            this.redisCacheService = redisCacheService;
        }

        public CacheVersion GetRightVersionOddsFeed(UserProfile profile)
        {
            const int maxDelayTimes = 3;
            var currentVersion = GetCurrentVersion(appSettings.Bucket.OddsFeed);
            var isFullChain = IsFullChain(appSettings.Bucket.OddsFeed);
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
            var currentVersion = GetCurrentVersion(appSettings.ResultFeed.Bucket);
            var isFullChain = IsFullChain(appSettings.ResultFeed.Bucket);
            var rightVersion = currentVersion - (delayTimes / appSettings.ResultFeed.Interval);
            if (isFullChain)
            {
                return rightVersion > -1 ? rightVersion : appSettings.ResultFeed.TotalVersion - Math.Abs(rightVersion);
            }

            return rightVersion > -1 ? rightVersion : -1;
        }

        private int GetRightVersionOddsFeed(int delayTime, int currentVersion, bool isFullChain)
        {
            var rightVersion = currentVersion - (delayTime / appSettings.Bucket.Interval);
            if (isFullChain)
            {
                return rightVersion > -1 ? rightVersion : appSettings.Bucket.TotalVersion - Math.Abs(rightVersion);
            }

            return rightVersion > -1 ? rightVersion : -1;
        }

        private int GetCurrentVersion(string bucket)
        {
            string currentVersionKey = CacheKeys.CurrentKey(appSettings.CachePrefix);
            var currentVersion = redisCacheService.Get<int>(string.Format("{0}:{1}", bucket, currentVersionKey));
            return currentVersion < 0 ? 0 : currentVersion;
        }

        private bool IsFullChain(string bucket)
        {
            string isFullChainKey = CacheKeys.CurrentKey(appSettings.CachePrefix);
            return redisCacheService.Get<bool>(string.Format("{0}:{1}", bucket, isFullChainKey));
        }
    }
}