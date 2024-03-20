namespace OddsFeed.AutoJobs.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Microsoft.AspNetCore.Hosting;
    using OddsFeed.Core.Configurations;
    using OddsFeed.Core;
    using OddsFeed.Core.Extensions;
    using OddsFeed.Core.Services;
    using OddsFeed.Models;
    using OddsFeed.Models.Matches;

    public class OddsfeedService : IOddsfeedService
    {
        private readonly CacheItemOptions cacheOptions = new() { RelativeExpiration = null };
        private readonly AppSettings appSettings;
        private readonly IRedisCacheService redisCacheService;
        private readonly ILogServiceProvider logServiceProvider;
        private readonly IOddsDataService oddsDataService;
        private readonly IWebHostEnvironment hostEnvironment;

        public OddsfeedService(
            AppSettings appSettings,
            IRedisCacheService redisCacheService,
            ILogServiceProvider logServiceProvider,
            IOddsDataService oddsDataService,
            IWebHostEnvironment hostEnvironment)
        {
            this.appSettings = appSettings;
            this.logServiceProvider = logServiceProvider;
            this.redisCacheService = redisCacheService;
            this.oddsDataService = oddsDataService;
            this.hostEnvironment = hostEnvironment;
        }

        public async Task Reset()
        {
            var oddsFeedBucket = appSettings.Bucket.OddsFeed;
            await StoreCache(oddsFeedBucket, appSettings.BetTypeSchemaKey, new List<BetTypeSchema>());
            await StoreCache(oddsFeedBucket, CacheKeys.CurrentKey(appSettings.CachePrefix), -1);
            await StoreCache(oddsFeedBucket, CacheKeys.FullChainKey(appSettings.CachePrefix), false);
        }

        public async Task Update()
        {
            var betTypeSchemas = await GetBetTypeSchemas();
            var liveMatches = await oddsDataService.GetLiveMatchAsync(betTypeSchemas);
            var deadBallMatches = await oddsDataService.GetDeadballMatchAsync(betTypeSchemas);

            var deadBallLength = deadBallMatches.Count;
            var splitIndex = deadBallLength / 2;
            var deadBalls1 = new List<Match>();
            var deadBalls2 = new List<Match>();

            for (var i = 0; i < splitIndex; i++)
            {
                deadBalls1.Add(deadBallMatches[i]);
            }

            for (var j = splitIndex; j < deadBallLength; j++)
            {
                deadBalls2.Add(deadBallMatches[j]);
            }

            var outrights = await oddsDataService.GetOutrightAsync();

            var currentVersion = await GetCurrentVersion() + 1;
            if (currentVersion == appSettings.Bucket.TotalVersion)
            {
                currentVersion = 0;
            }
            var versionKey = currentVersion.ToString();

            await StoreCache(appSettings.Bucket.OddsFeed, CacheKeys.CurrentKey(appSettings.CachePrefix), currentVersion);

            await StoreCache(appSettings.Bucket.LiveMatches, versionKey, liveMatches);
            await StoreCache(appSettings.Bucket.DeadMatches, versionKey + "_1", deadBalls1);
            await StoreCache(appSettings.Bucket.DeadMatches, versionKey + "_2", deadBalls2);
            await StoreCache(appSettings.Bucket.Outrights, versionKey, outrights);

            if (currentVersion == appSettings.Bucket.TotalVersion - 1)
            {
                await StoreCache(appSettings.Bucket.OddsFeed, CacheKeys.FullChainKey(appSettings.CachePrefix), true);
            }
        }

        private async Task<IList<BetTypeSchema>> GetBetTypeSchemas()
        {
            var rediskey = string.Format(Constants.RedisKeyFormat, appSettings.Bucket.OddsFeed, appSettings.BetTypeSchemaKey);
            var document = await redisCacheService.GetAsync<IList<BetTypeSchema>>(rediskey);

            if (document == null || document.Count == 0)
            {
                var newValue = ReadBetTypeFromFile();

                if (newValue != null)
                {
                    rediskey = string.Format(Constants.RedisKeyFormat, appSettings.Bucket.OddsFeed, appSettings.BetTypeSchemaKey);
                    await redisCacheService.SetAsync(rediskey, newValue, cacheOptions);
                    return newValue;
                }

                return new List<BetTypeSchema>();
            }

            return document;
        }

        private IList<BetTypeSchema> ReadBetTypeFromFile()
        {
            var filePath = Path.Combine(hostEnvironment.ContentRootPath, appSettings.BetTypeSchemaPath);
            var values = ReadJsonFromFile.GetBetTypeSchemas<BetTypeSchema>(filePath);
            return values;
        }

        private async Task<int> GetCurrentVersion()
        {
            var rediskey = string.Format(Constants.RedisKeyFormat, appSettings.Bucket.OddsFeed, CacheKeys.CurrentKey(appSettings.CachePrefix));
            var version = await redisCacheService.GetAsync<int>(rediskey);
            return version;
        }

        private async Task StoreCache<T>(string bucketName, string key, T value)
        {
            try
            {
                var rediskey = string.Format(Constants.RedisKeyFormat, bucketName, key);
                await redisCacheService.SetAsync(rediskey, value, cacheOptions);
            }
            catch (Exception ex)
            {
                await logServiceProvider.LogSentryAsync(ex);
            }
        }
    }
}