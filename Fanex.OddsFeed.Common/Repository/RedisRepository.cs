using System;
using Fanex.Caching;
using Newtonsoft.Json;

namespace Fanex.OddsFeed.Common.Repository
{
    public static class RedisRepository
    {
        public static T Get<T>(string key)
        {
            return DeepClone(ConfiguredCacheService.Get<T>(key));
        }

        public static void Set<T>(string key, T data, TimeSpan? expire = null)
        {
            if (expire != null)
            {
                ConfiguredCacheService.Set(key, DeepClone(data), new CacheItemOptions().SetSlidingExpiration(expire.Value));
            }
            else
            {
                ConfiguredCacheService.Set(key, DeepClone(data));
            }
        }

        public static void Delete(string key)
        {
            ConfiguredCacheService.Remove(key);
        }

        private static T DeepClone<T>(T source)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source));
        }
    }
}