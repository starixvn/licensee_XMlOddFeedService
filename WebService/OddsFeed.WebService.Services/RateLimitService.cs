namespace OddsFeed.WebService.Services
{
    using System;
    using Fanex.Caching;
    using OddsFeed.Core.Constants;
    using OddsFeed.Core.Services;
    using OddsFeed.WebService.Models;
    using OddsFeed.WebService.Models.FilterAttribute;
    using OddsFeed.WebService.Services.Interfaces;

    public class RateLimitService : IRateLimitService
    {
        private readonly WebSettings webSettings;
        private readonly IRedisCacheService redisCacheService;

        public RateLimitService(WebSettings webSettings, IRedisCacheService redisCacheService)
        {
            this.webSettings = webSettings;
            this.redisCacheService = redisCacheService;
        }

        public ThrottleInfo GetThrottleInfo(string ipAdress)
        {
            var throttleInfo = redisCacheService.Get<ThrottleInfo>(string.Format(Constants.RedisKeyFormat, webSettings.Bucket.Throttler, UpdateIPText(ipAdress)));

            return throttleInfo ?? new ThrottleInfo();
        }

        public void UpdateThrottleInfo(string ipAdress, ThrottleInfo throttleInfo)
        {
            redisCacheService.Set(string.Format(Constants.RedisKeyFormat, webSettings.Bucket.Throttler, UpdateIPText(ipAdress)), throttleInfo);
        }

        public void AddToBlockList(string ipAdress, DateTime expiredAt)
        {
            var cacheItemOptions = new CacheItemOptions
            {
                RelativeExpiration = TimeSpan.FromSeconds(webSettings.Throttler.BlockTimeInSeconds)
            };
            redisCacheService.Set(string.Format(Constants.RedisKeyFormat, webSettings.Bucket.BlockList, UpdateIPText(ipAdress)), expiredAt, cacheItemOptions);
        }

        public bool IsBlocked(string ipAdress)
        {
            var isBlocked = true;
            var expiredTime = redisCacheService.Get<DateTime>(string.Format(Constants.RedisKeyFormat, webSettings.Bucket.BlockList, UpdateIPText(ipAdress)));

            var curTime = DateTime.Now;

            if (curTime >= expiredTime)
            {
                redisCacheService.Remove(string.Format(Constants.RedisKeyFormat, webSettings.Bucket.BlockList, UpdateIPText(ipAdress)));
                isBlocked = false;
            }

            return isBlocked;
        }

        private static string UpdateIPText(string ipAdress)
            => ipAdress.Replace(":", "_");
    }
}