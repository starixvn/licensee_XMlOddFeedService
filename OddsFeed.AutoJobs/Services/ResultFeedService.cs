namespace OddsFeed.AutoJobs.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using OddsFeed.Core.Configurations;
    using OddsFeed.Core.Constants;
    using OddsFeed.Core.Extensions;
    using OddsFeed.Core.Services;
    using OddsFeed.Models.Matches;

    public class ResultFeedService : IResultFeedService
    {
        private readonly CacheItemOptions cacheOptions = new() { SlidingExpiration = TimeSpan.FromHours(24) };
        private readonly AppSettings appSettings;
        private readonly IRedisCacheService redisCacheService;
        private readonly ILogServiceProvider logServiceProvider;
        private readonly IResultFeedDataService resultFeedDataService;

        public ResultFeedService(AppSettings appSettings,
            IRedisCacheService redisCacheService,
            ILogServiceProvider logServiceProvider,
            IResultFeedDataService resultFeedDataService)
        {
            this.appSettings = appSettings;
            this.redisCacheService = redisCacheService;
            this.logServiceProvider = logServiceProvider;
            this.resultFeedDataService = resultFeedDataService;
        }

        public async Task Reset()
        {
            await redisCacheService.SetAsync(string.Format(appSettings.ResultFeed.Bucket, CacheKeys.CurrentKey(appSettings.CachePrefix)), -1);
            await redisCacheService.SetAsync(string.Format(appSettings.ResultFeed.Bucket, CacheKeys.FullChainKey(appSettings.CachePrefix)), false);
        }

        public async Task Update()
        {
            int currentVersion = await GetCurrentVersion() + 1;
            if (currentVersion == appSettings.ResultFeed.TotalVersion)
            {
                currentVersion = 0;
            }
            var versionKey = currentVersion.ToString();
            IList<Match> matches = await resultFeedDataService.GetMatchResultDetailsAsync(DateTime.Today);

            await StoreCache(appSettings.ResultFeed.Bucket, CacheKeys.CurrentKey(appSettings.CachePrefix), currentVersion);
            await StoreCache(appSettings.ResultFeed.BucketData, versionKey, matches);

            if (currentVersion == appSettings.ResultFeed.TotalVersion - 1)
            {
                await StoreCache(appSettings.ResultFeed.Bucket, CacheKeys.FullChainKey(appSettings.CachePrefix), true);
            }
        }

        private async Task<int> GetCurrentVersion()
        => await redisCacheService.GetAsync<int>(string.Format(Constants.RedisKeyFormat,
            appSettings.ResultFeed.Bucket,
            CacheKeys.CurrentKey(appSettings.CachePrefix)));

        private async Task StoreCache<T>(string bucketName, string key, T value)
        {
            try
            {
                await redisCacheService.SetAsync(string.Format(Constants.RedisKeyFormat, bucketName, key), value, cacheOptions);
            }
            catch (Exception ex)
            {
                await logServiceProvider.LogSentryAsync(ex);
            }
        }
    }
}