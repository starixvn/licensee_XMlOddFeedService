namespace Fanex.OddsFeed.AutoJobs.Services
{
    using Fanex.Caching;
    using Fanex.OddsFeed.Core.Configurations;
    using Fanex.OddsFeed.Core.Constants;
    using Fanex.OddsFeed.Core.Extensions;
    using Fanex.OddsFeed.Core.Services;
    using Fanex.OddsFeed.Models;
    using Fanex.OddsFeed.Models.Matches;
    using Microsoft.AspNetCore.Hosting;

    public class OddsfeedService : IOddsfeedService
    {
        private readonly CacheItemOptions cacheOptions = new() { RelativeExpiration = null };
        private readonly AppSettings appSettings;
        private readonly ICacheService cacheService;
        private readonly IRedisCacheService redisCacheService;
        private readonly ILogServiceProvider logServiceProvider;
        private readonly IOddsDataService oddsDataService;
        private readonly IWebHostEnvironment hostEnvironment;

        public OddsfeedService(AppSettings appSettings,
            ICacheService cacheService,
            IRedisCacheService redisCacheService,
            ILogServiceProvider logServiceProvider,
            IOddsDataService oddsDataService,
            IWebHostEnvironment hostEnvironment)
        {
            this.appSettings = appSettings;
            this.cacheService = cacheService;
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

            var outrights = oddsDataService.GetOutrightAsync();

            var currentVersion = await GetCurrentVersion() + 1;
            if (currentVersion == appSettings.TotalVersion)
            {
                currentVersion = 0;
            }
            var versionKey = currentVersion.ToString();

            await StoreCache(appSettings.Bucket.OddsFeed, CacheKeys.CurrentKey(appSettings.CachePrefix), currentVersion);

            await StoreCache(appSettings.Bucket.LiveMatches, versionKey, liveMatches);
            await StoreCache(appSettings.Bucket.DeadMatches, versionKey + "_1", deadBalls1);
            await StoreCache(appSettings.Bucket.DeadMatches, versionKey + "_2", deadBalls2);
            await StoreCache(appSettings.Bucket.Outrights, versionKey, outrights);

            if (currentVersion == appSettings.TotalVersion - 1)
            {
                await StoreCache(appSettings.Bucket.OddsFeed, CacheKeys.FullChainKey(appSettings.CachePrefix), true);
            }
        }

        private async Task<IList<BetTypeSchema>> GetBetTypeSchemas()
        {
            var document = await redisCacheService.GetAsync<IList<BetTypeSchema>>(string.Format(Constants.RedisKeyFormat,
                appSettings.Bucket.OddsFeed,
                appSettings.BetTypeSchemaKey));

            if (document == null || document.Count == 0)
            {
                var newValue = ReadBetTypeFromFile();

                if (newValue != null)
                {
                    await redisCacheService.SetAsync(string.Format(Constants.RedisKeyFormat, appSettings.Bucket.OddsFeed, appSettings.BetTypeSchemaKey), newValue, cacheOptions);
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
            => await cacheService.GetAsync<int>(string.Format(Constants.RedisKeyFormat, appSettings.Bucket.OddsFeed, CacheKeys.CurrentKey(appSettings.CachePrefix)));

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