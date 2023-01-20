namespace Fanex.OddsFeed.Web.Services.Extensions
{
    using Fanex.OddsFeed.Web.Services.Services;
    using Fanex.OddsFeed.Web.Services.Services.Interfaces;
    using Microsoft.Extensions.DependencyInjection;

    public static class DI
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IVersionCachingService, VersionCachingService>();
            services.AddSingleton<IRateLimitService, RateLimitService>();
            services.AddSingleton<IFeedCachingOperator, FeedCachingOperator>();
        }
    }
}