namespace Fanex.OddsFeed.WebServiceLibrary.Caching
{
    using System;
    using Common.Repository;
    using Fanex.OddsFeed.Common;
    using Fanex.OddsFeed.Common.Configurations;
    using Fanex.OddsFeed.Common.FilterAtribute;

    public class RateLimitService : IRateLimitService
    {
        private static readonly string throttlerBucket = AppSettings.BucketThrottler;
        private static readonly string blocklistBucket = AppSettings.BucketBlockList;
        private static readonly int blockTimeInSeconds = int.Parse(AppSettings.ThrollerBlockTimeInSeconds);

        public ThrottleInfo GetThrottleInfo(string ipAdress)
        {
            var throttleInfo = RedisRepository.Get<ThrottleInfo>(string.Format(Constants.RedisKeyFormat, throttlerBucket, UpdateIPText(ipAdress)));

            return throttleInfo ?? new ThrottleInfo();
        }

        public void UpdateThrottleInfo(string ipAdress, ThrottleInfo throttleInfo)
        {
            RedisRepository.Set<ThrottleInfo>(string.Format(Constants.RedisKeyFormat, throttlerBucket, UpdateIPText(ipAdress)), throttleInfo);
        }

        public void AddToBlockList(string ipAdress, DateTime expiredAt)
        {
            RedisRepository.Set<DateTime>(string.Format(Constants.RedisKeyFormat, blocklistBucket, UpdateIPText(ipAdress)), expiredAt, TimeSpan.FromSeconds(blockTimeInSeconds));
        }

        public bool IsBlocked(string ipAdress)
        {
            var isBlocked = true;

            var expiredTime = RedisRepository.Get<DateTime>(string.Format(Constants.RedisKeyFormat, blocklistBucket, UpdateIPText(ipAdress)));

            var curTime = DateTime.Now;

            if (curTime >= expiredTime)
            {
                RedisRepository.Delete(string.Format(Constants.RedisKeyFormat, blocklistBucket, UpdateIPText(ipAdress)));
                isBlocked = false;
            }

            return isBlocked;
        }

        private static string UpdateIPText(string ipAdress)
        {
            return ipAdress.Replace(":", "_");
        }
    }
}