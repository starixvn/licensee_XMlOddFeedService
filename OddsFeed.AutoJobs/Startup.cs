namespace OddsFeed.AutoJobs
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Caching.Redis;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.AspNetCore;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using OddsFeed.AutoJobs.Mapping;
    using OddsFeed.AutoJobs.Models;
    using OddsFeed.AutoJobs.Services;
    using OddsFeed.AutoJobs.Workers;
    using OddsFeed.Core;
    using OddsFeed.Core.Configurations;
    using OddsFeed.Core.Services;
    using OddsFeed.Services;
    using ONEbook.UM.AspNetCore;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.Services;
    using ONEbook.UM.Transport;
    using Sentry;
    using StackExchange.Redis;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            LogManager
                .SetDefaultLogCategory(Configuration.GetSection("Fanex.Logging:LogCategory").Value)
                .Use(new List<ILogEngine>
                {
                new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(Configuration.GetSection("Fanex.Logging:SentryEngineOptions:Dsn").Value)
                })
                });

            var connections = Configuration.GetSection("ConnectionStrings")
                .GetChildren()
                .AsEnumerable()
                .Where(x => !string.IsNullOrWhiteSpace(x.GetValue<string>("Name")))
                .ToDictionary(x => x.GetValue<string>("Name"),
                    y => new ConnectionConfiguration(y.GetValue<string>("Name"), y.GetValue<string>("ConnectionString"), y.GetValue<string>("ProviderName")));

            DbSettingProviderManager
                .StartNewSession()
                .Use(connections)
                .WithSqlServer()
                .UseDefaultDbSettingProvider("~/App_Data/SpNames.xml")
                .Run();

            ExceptionHandler
                .UseConfig(new LoggingConfiguration())
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            services.AddSingleton(Configuration);
            services.AddControllers();
            ConfigDataProtection(services, Configuration);
            AddDI(services);
            AddConfiguration(services, Configuration);
            AddWorkers(services);
        }

        private static void AddConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
            services.AddSingleton(appSettings);

            var jobSettings = configuration.GetSection("JobSettings").Get<List<JobSetting>>();
            services.AddSingleton(jobSettings);
        }

        private static void AddWorkers(IServiceCollection services)
        {
            services.AddHostedService<OddsfeedWorker>();
            services.AddHostedService<ResultFeedWorker>();
        }

        private void AddDI(IServiceCollection services)
        {
            #region ONEbookUM

            services.AddSingleton(new UMClientConfiguration().Initialize(Configuration));
            services.AddSingleton<IUMCacheService, UMCacheService>();
            services.AddSingleton<IUMRestClient, UMRestClient>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IHttpContextHelper, HttpContextHelper>();
            services.AddSingleton<IMaintenanceService, MaintenanceService>();

            #endregion ONEbookUM

            services.AddSingleton<IDynamicRepository, DynamicRepository>();

            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<ILogServiceProvider, LogServiceProvider>();
            services.AddSingleton<IRedisCacheService, RedisCacheService>();
            services.AddSingleton<ILogger, Logger>();

            services.AddSingleton<IMatchMapping, MatchMapping>();
            services.AddSingleton<IOutrightMapping, OutrightMapping>();

            services.AddSingleton<IOddsDataService, OddsDataService>();
            services.AddSingleton<IOddsfeedService, OddsfeedService>();
            services.AddSingleton<IResultFeedService, ResultFeedService>();
            services.AddSingleton<IResultFeedDataService, ResultFeedDataService>();
        }

        private static void ConfigDataProtection(IServiceCollection services, IConfiguration configuration)
        {
            services.AddRedisCache(configuration.GetSection("RedisConfig"));

            var redis = ConnectionMultiplexer.Connect(configuration.GetValue<string>("RedisConfig:ConnectionString"));
            var appName = configuration.GetValue<string>("RedisConfig:ApplicationName");
            services.AddDataProtection()
                .SetApplicationName(appName)
                .PersistKeysToStackExchangeRedis(redis, $"{appName}:DataProtectionKeys");
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ErrorHandler>();
                app.UseHsts();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Whoami}/{action=Index}/{id?}");
            });
        }
    }
}