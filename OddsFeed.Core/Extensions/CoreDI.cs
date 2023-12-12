namespace OddsFeed.Core.Extensions
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using OddsFeed.Core.Services;
    using ONEbook.UM.AspNetCore;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.Services;
    using ONEbook.UM.Transport;

    public static class CoreDI
    {
        public static void RegistryCoreServices(IServiceCollection services, IConfiguration configuration)
        {
            #region Core repository

            services.AddSingleton<IDynamicRepository, DynamicRepository>();

            #endregion Core repository

            #region ONEbookUM

            services.AddSingleton(new UMClientConfiguration().Initialize(configuration));
            services.AddSingleton<IHttpContextHelper, HttpContextHelper>();
            services.AddSingleton<IUMCacheService, UMCacheService>();
            services.AddSingleton<IUMRestClient, UMRestClient>();
            services.AddSingleton<IMaintenanceService, MaintenanceService>();

            #endregion ONEbookUM

            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<ILogServiceProvider, LogServiceProvider>();
            services.AddSingleton<IRedisCacheService, RedisCacheService>();
            services.AddSingleton<ILogger, Logger>();
        }
    }
}