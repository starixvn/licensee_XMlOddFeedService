namespace Fanex.OddsFeed.Web.Services.Services
{
    using System;
    using Fanex.Caching;
    using Fanex.OddsFeed.Core.Services;
    using Fanex.OddsFeed.Web.Services.Models;
    using Fanex.OddsFeed.Web.Services.Models.FilterAttribute;
    using Fanex.OddsFeed.Web.Services.Services.Interfaces;

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
            var throttleInfo = redisCacheService.Get<ThrottleInfo>(string.Format(Core.Constants.Constants.RedisKeyFormat,
                webSettings.Bucket.Throttler,
                UpdateIPText(ipAdress)));

            return throttleInfo ?? new ThrottleInfo();
        }

        public void UpdateThrottleInfo(string ipAdress, ThrottleInfo throttleInfo)
        {
            redisCacheService.Set(string.Format(Core.Constants.Constants.RedisKeyFormat, webSettings.Bucket.Throttler, UpdateIPText(ipAdress)), throttleInfo);
        }

        public void AddToBlockList(string ipAdress, DateTime expiredAt)
        {
            redisCacheService.Set<DateTime>(string.Format(Core.Constants.Constants.RedisKeyFormat, webSettings.Bucket.BlockList, UpdateIPText(ipAdress)), expiredAt,
                new CacheItemOptions
                {
                    RelativeExpiration = TimeSpan.FromSeconds(webSettings.Throttler.BlockTimeInSeconds)
                });
        }

        public bool IsBlocked(string ipAdress)
        {
            var isBlocked = true;

            var expiredTime = redisCacheService.Get<DateTime>(string.Format(Core.Constants.Constants.RedisKeyFormat, webSettings.Bucket.BlockList, UpdateIPText(ipAdress)));

            var curTime = DateTime.Now;

            if (curTime >= expiredTime)
            {
                redisCacheService.Remove(string.Format(Core.Constants.Constants.RedisKeyFormat, webSettings.Bucket.BlockList, UpdateIPText(ipAdress)));
                isBlocked = false;
            }

            return isBlocked;
        }

        private static string UpdateIPText(string ipAdress)
            => ipAdress.Replace(":", "_");
    }
}