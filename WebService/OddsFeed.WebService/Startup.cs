namespace OddsFeed.WebService
{
    using System;
    using System.Collections.Generic;
    using Fanex.Caching.Redis;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.Logging.Sentry;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Net.Http.Headers;
    using OddsFeed.Core;
    using OddsFeed.Core.Services;
    using OddsFeed.Services;
    using OddsFeed.WebService.Core.Builders;
    using OddsFeed.WebService.Core.Filters;
    using OddsFeed.WebService.Extensions;
    using OddsFeed.WebService.Filters;
    using OddsFeed.WebService.Models;
    using OddsFeed.WebService.Services;
    using OddsFeed.WebService.Services.Interfaces;
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

            services.AddFanexData(Configuration);

            services.AddSingleton(Configuration);
            services.AddControllersWithViews(x => x.EnableEndpointRouting = false);
            services.AddSession();
            services.AddResponseCaching();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            ConfigDataProtection(services, Configuration);
            services.AddSingleton<RateLimitFilterAttribute>();
            AddConfiguration(services, Configuration);
            AddDI(services);
        }

        private static void AddConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            var webSettings = configuration.GetSection("AppSettings").Get<WebSettings>();
            services.AddSingleton(webSettings);
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

            services.AddSingleton<IMatchBuilder, MatchBuilder>();
            services.AddSingleton<IOutrightBuilder, OutrightBuilder>();
            services.AddSingleton<IEventBuilder, EventBuilder>();

            services.AddSingleton<IMatchFilter, MatchFilter>();
            services.AddSingleton<IOutrightFilter, OutrightFilter>();

            services.AddSingleton<IBetDaqService, BetDaqService>();
            services.AddSingleton<IBetRadarService, BetRadarService>();
            services.AddSingleton<IBetTypeService, BetTypeService>();
            services.AddSingleton<IEventFeedService, EventFeedService>();
            services.AddSingleton<IFeedCachingOperator, FeedCachingOperator>();
            services.AddSingleton<IOddsFeedService, OddsFeedService>();
            services.AddSingleton<IPartnerService, PartnerService>();
            services.AddSingleton<IRateLimitService, RateLimitService>();
            services.AddSingleton<IResultFeedService, ResultFeedService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IVersionCachingService, VersionCachingService>();
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
            }

            app.UseHsts();
            app.UseStaticFiles();
            app.UseSession();
            app.UseResponseCaching();
            app.UseForwardedHeaders();

            app.Use(async (ctx, next) =>
            {
                if (ctx.Response.StatusCode == 404)
                {
                    ctx.Request.Path = "/Errors/PageNotFound";
                    await next();
                }

                ctx.Request.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = TimeSpan.FromSeconds(60)
                };
                await next();
            });

            var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
            string feedUrlPath = configuration.GetValue<string>("AppSettings:FeedUrlPath");
            if (!string.IsNullOrWhiteSpace(feedUrlPath))
            {
                feedUrlPath = "/";
            }
            
            app.RegisterXMLOddsfeedRoutes(feedUrlPath);
        }
    }
}