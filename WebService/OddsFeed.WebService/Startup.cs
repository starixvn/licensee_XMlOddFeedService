namespace OddsFeed.WebService
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Fanex.Caching.Redis;
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
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Net.Http.Headers;
    using NUglify.Css;
    using NUglify.JavaScript;
    using OddsFeed.Core;
    using OddsFeed.Core.Mapping;
    using OddsFeed.Core.Services;
    using OddsFeed.WebService.Core.Builders;
    using OddsFeed.WebService.Core.Filters;
    using OddsFeed.WebService.Extensions;
    using OddsFeed.WebService.Filters;
    using OddsFeed.WebService.Models;
    using OddsFeed.WebService.Services;
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

            services.AddFanexSql(Configuration);
            
            ExceptionHandler
                .UseConfig(new LoggingConfiguration())
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            services.AddSingleton(Configuration);
            services.AddControllersWithViews(x => x.EnableEndpointRouting = false);
            
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddSession();
            services.AddResponseCaching();
            
            ConfigDataProtection(services, Configuration);
            services.AddSingleton<RateLimitFilterAttribute>();
            AddConfiguration(services, Configuration);
            AddDI(services);

            services.AddWebOptimizer(pipeline => {
                var cssSetting = new CssSettings { OutputMode = NUglify.OutputMode.MultipleLines, CommentMode = CssComment.None };
                var scriptSetting = new CodeSettings { MinifyCode = true };

                pipeline.AddCssBundle(
                    "/minify/css/help.min.css",
                    cssSetting,
                    "css/shCore.css",
                    "css/shThemeDefault.css",
                    "css/Help/menu.css",
                    "css/Help/content.css");
                pipeline.AddCssBundle(
                    "/minify/css/404.min.css",
                    cssSetting,
                    "css/404.css");

                pipeline.AddJavaScriptBundle(
                    "/minify/js/help.min.js",
                    scriptSetting,
                    "js/jquery-latest.js",
                    "js/shCore.js",
                    "js/shBrushXml.js",
                    "js/Help/menu.js");
            });
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

            services.AddSingleton<IMatchMapping, MatchMapping>();

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

            const short WorkerThreads = 100;
            const short CompletionPortThreads = 100;
            _ = ThreadPool.SetMinThreads(WorkerThreads, CompletionPortThreads);
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
            var feedUrlPath = configuration.GetValue<string>("AppSettings:FeedUrlPath");
            var subPath = configuration.GetValue<string>("AppSettings:VirtualPathWebService");

            if (!string.IsNullOrWhiteSpace(subPath))
            {
                app.UsePathBase(subPath);
            }

            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ErrorHandler>();
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Errors/{0}");
            app.UseHttpsRedirection();
            app.UseWebOptimizer();
            app.UseStaticFiles();
            app.UseSession();
            app.UseResponseCaching();

            app.Use(async (ctx, next) =>
            {
                if (!string.IsNullOrWhiteSpace(subPath))
                {
                    ctx.Request.PathBase = subPath;
                }

                ctx.Request.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromSeconds(60)
                };
                await next();
            });

            if (!string.IsNullOrWhiteSpace(feedUrlPath))
            {
                feedUrlPath += "/";
            }

            app.RegisterXMLOddsfeedRoutes(feedUrlPath);
        }
    }
}