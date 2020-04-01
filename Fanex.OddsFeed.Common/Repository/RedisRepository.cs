using Fanex.OddsFeed.Common.Helpers;
using Newtonsoft.Json;
using System;

namespace Fanex.OddsFeed.Common.Repository
{
    public static class RedisRepository
    {
        public static T ReadData<T>(string key)
        {
            var cache = MyRedisConnectorHelper
               .RedisConnection.GetDatabase();
            var value = cache.StringGet(GetKey(key));
            return value.IsNullOrEmpty ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public static bool StoreData<T>(string key, T data, TimeSpan? expire = null)
        {
            var cache = MyRedisConnectorHelper.RedisConnection
               .GetDatabase();

            return cache.StringSet(GetKey(key), JsonConvert.SerializeObject(data), expire);
        }

        public static bool Remove(string key)
        {
            var cache = MyRedisConnectorHelper.RedisConnection
               .GetDatabase();
            return cache.KeyDelete(GetKey(key));
        }

        private static string GetKey(string key)
        {
            return string.Format("{0}:{1}", "XMLOddsFeedService", key);
        }
    }
}
