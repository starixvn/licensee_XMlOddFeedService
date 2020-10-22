namespace Fanex.OddsFeed.WebServiceLibrary.Caching
{
    using Common.Repository;
    using Fanex.OddsFeed.Common;
    using Fanex.OddsFeed.Common.Configurations;
    using Fanex.OddsFeed.Common.FilterAtribute;
    using System;

    public class RateLimitService : IRateLimitService
    {
        private static readonly string throttlerBucket = AppSettings.BucketThrottler;
        private static readonly string blocklistBucket = AppSettings.BucketBlockList;
        private static readonly int blockTimeInSeconds = int.Parse(AppSettings.ThrollerBlockTimeInSeconds);

        public ThrottleInfo GetThrottleInfo(string ipAdress)
        {
            var throttleInfo = RedisRepository.ReadData<ThrottleInfo>(string.Format(Constants.RedisKeyFormat, throttlerBucket, UpdateIPText(ipAdress)));

            return throttleInfo ?? new ThrottleInfo();
        }

        public void UpdateThrottleInfo(string ipAdress, ThrottleInfo throttleInfo)
        {
            RedisRepository.StoreData<ThrottleInfo>(string.Format(Constants.RedisKeyFormat, throttlerBucket, UpdateIPText(ipAdress)), throttleInfo);
        }

        public void AddToBlockList(string ipAdress, DateTime expiredAt)
        {
            RedisRepository.StoreData<DateTime>(string.Format(Constants.RedisKeyFormat, blocklistBucket, UpdateIPText(ipAdress)), expiredAt, TimeSpan.FromSeconds(blockTimeInSeconds));
        }

        public bool IsBlocked(string ipAdress)
        {
            var isBlocked = true;

            var expiredTime = RedisRepository.ReadData<DateTime>(string.Format(Constants.RedisKeyFormat, blocklistBucket, UpdateIPText(ipAdress)));

            var curTime = DateTime.Now;

            if (curTime >= expiredTime)
            {
                RedisRepository.Remove(string.Format(Constants.RedisKeyFormat, blocklistBucket, UpdateIPText(ipAdress)));
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