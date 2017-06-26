namespace Fanex.OddsFeed.Common.Helpers
{
    using Configurations;

    public static class SpHelper
    {
        public static string RealSp(string sp)
        {
            return AppSettings.IsSpLive ? sp : sp +  AppSettings.SpSuffix;
        }
    }
}
