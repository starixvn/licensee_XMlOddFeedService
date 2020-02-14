using Fanex.OddsFeed.Common.Helpers;
using Newtonsoft.Json;

namespace Fanex.OddsFeed.Common.Repository
{
    public static class RedisRepository
    {
        public static T ReadData<T>(string key)
        {
            var cache = MyRedisConnectorHelper
               .RedisConnection.GetDatabase();
            var value = cache.StringGet(key);
            return value.IsNullOrEmpty ? default(T) : JsonConvert.DeserializeObject<T>(cache.StringGet(key));
        }

        public static bool StoreData<T>(string key, T data)
        {
            var cache = MyRedisConnectorHelper.RedisConnection
               .GetDatabase();

            return cache.StringSet(key, JsonConvert.SerializeObject(data));
        }
    }
}
