namespace Fanex.OddsFeed.Core.Services
{
    using Fanex.Caching;
    using Newtonsoft.Json;

    public interface IRedisCacheService
    {
        T Get<T>(string key);

        Task<T> GetAsync<T>(string key);

        Task SetAsync<T>(string key, T value, CacheItemOptions? options = null);

        void Set<T>(string key, T value, CacheItemOptions? options = null);

        void Remove(string key);

        Task RemoveAsync(string key);
    }

    public class RedisCacheService : IRedisCacheService
    {
        private readonly ICacheService cacheService;

        public RedisCacheService(ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }

        public T Get<T>(string key)
        {
            var valueOnCache = cacheService.Get<string>(key);
            if (!string.IsNullOrWhiteSpace(valueOnCache))
            {
                return JsonConvert.DeserializeObject<T>(valueOnCache);
            }
            
            return default;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var valueOnCache = await cacheService.GetAsync<string>(key);
            if (!string.IsNullOrWhiteSpace(valueOnCache))
            {
                return JsonConvert.DeserializeObject<T>(valueOnCache);
            }

            return default;
        }

        public void Remove(string key)
        {
            cacheService.Remove(key);
        }

        public async Task RemoveAsync(string key)
        {
            await cacheService.RemoveAsync(key);
        }

        public void Set<T>(string key, T value, CacheItemOptions? options = null)
        {
            var json = JsonConvert.SerializeObject(value, Formatting.Indented);
            if (options != null)
            {
                cacheService.Set(key, json, options);
            }
            else
            {
                cacheService.Set(key, json);
            }
        }

        public async Task SetAsync<T>(string key, T value, CacheItemOptions? options = null)
        {
            var json = JsonConvert.SerializeObject(value, Formatting.Indented);
            if (options != null)
            {
                await cacheService.SetAsync(key, json, options);
            }
            else
            {
                await cacheService.SetAsync(key, json);
            }
        }
    }
}