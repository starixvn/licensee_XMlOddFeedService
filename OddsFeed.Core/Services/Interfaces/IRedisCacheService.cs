namespace OddsFeed.Core.Services
{
    using System.Threading.Tasks;
    using Fanex.Caching;

    public interface IRedisCacheService
    {
        T Get<T>(string key);

        Task<T> GetAsync<T>(string key);

        Task SetAsync<T>(string key, T value, CacheItemOptions options = null);

        void Set<T>(string key, T value, CacheItemOptions options = null);

        void Remove(string key);

        Task RemoveAsync(string key);
    }
}