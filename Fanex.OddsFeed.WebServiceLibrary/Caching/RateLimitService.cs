namespace Fanex.OddsFeed.WebServiceLibrary.Caching
{
    using System;
    using Couchbase;
    using Fanex.OddsFeed.Common.Configurations;
    using Fanex.OddsFeed.Common.FilterAtribute;

    public class RateLimitService : IRateLimitService
    {
        private static readonly string throttlerBucket = AppSettings.BucketThrottler;
        private static readonly string blocklistBucket = AppSettings.BucketBlockList;
        private static readonly int blockTimeInSeconds = int.Parse(AppSettings.ThrollerBlockTimeInSeconds);

        public ThrottleInfo GetThrottleInfo(string ipAdress)
        {
            var bucket = ClusterHelper.GetBucket(throttlerBucket);
            var throttleInfo = bucket.Get<ThrottleInfo>(ipAdress).Value;

            return throttleInfo ?? new ThrottleInfo();
        }

        public void UpdateThrottleInfo(string ipAdress, ThrottleInfo throttleInfo)
        {
            var bucket = ClusterHelper.GetBucket(throttlerBucket);
            bucket.Upsert(ipAdress, throttleInfo);
        }

        public void AddToBlockList(string ipAdress, DateTime expiredAt)
        {
            var bucket = ClusterHelper.GetBucket(blocklistBucket);
            bucket.Upsert(ipAdress, expiredAt, TimeSpan.FromSeconds(blockTimeInSeconds));
        }

        public bool IsBlocked(string ipAdress)
        {
            var isBlocked = true;

            var bucket = ClusterHelper.GetBucket(blocklistBucket);
            var expiredTime = bucket.Get<DateTime>(ipAdress).Value;

            var curTime = DateTime.Now;

            if (curTime >= expiredTime)
            {
                bucket.Remove(ipAdress);
                isBlocked = false;
            }

            return isBlocked;
        }
    }
}