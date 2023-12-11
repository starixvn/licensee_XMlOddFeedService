namespace OddsFeed.Core.Extensions
{
    public static class CacheKeys
    {
        public static string CurrentKey(string cachePrefix) => $"{cachePrefix}current";

        public static string FullChainKey(string cachePrefix) => $"{cachePrefix}fullchain";
    }
}