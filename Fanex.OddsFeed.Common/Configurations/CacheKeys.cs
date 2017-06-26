namespace Fanex.OddsFeed.Common.Configurations
{
    public static class CacheKeys
    {
        public static string CurrentKey(string cachePrefix) => $"{cachePrefix}current";

        public static string FullChainKey(string cachePrefix) => $"{cachePrefix}fullchain";
    }
}
