using Fanex.Caching.Redis;
using Fanex.Data;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.AspNetCore;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Sentry;
using Fanex.OddsFeed.Core;
using Fanex.OddsFeed.Core.Extensions;
using Fanex.OddsFeed.Core.Models.Email;
using Fanex.OddsFeed.Repository;
using Fanex.OddsFeed.Repository.Interfaces;
using Fanex.OddsFeed.Web.Extensions;
using Fanex.OddsFeed.Web.Filters;
using Fanex.OddsFeed.Web.Services.Models;
using Fanex.OddsFeed.Web.Services.Repositories;
using Fanex.OddsFeed.Web.Services.Repositories.Interfaces;
using Fanex.OddsFeed.Web.Services.Services;
using Fanex.OddsFeed.Web.Services.Services.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
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

        Dictionary<string, string> connectionString = Configuration
                .GetSection("ConnectionStrings")
                .GetChildren()
                .ToDictionary(connection => connection.Key, connection => connection.Value, StringComparer.OrdinalIgnoreCase);

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
        services.AddControllersWithViews(x => x.EnableEndpointRouting = false);
        services.AddSession();
        services.AddResponseCaching();
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });
        ConfigDataProtection(services, Configuration);
        services.AddSingleton<RateLimitFilterAttribute>();
        CoreDI.RegistryCoreServices(services, Configuration);
        AddConfiguration(services, Configuration);
        AddRepositories(services);
        AddServices(services);
    }

    private void AddConfiguration(IServiceCollection services, IConfiguration configuration)
    {
        var webSettings = configuration.GetSection("AppSettings").Get<WebSettings>();
        services.AddSingleton(webSettings);

        var emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();
        services.AddSingleton(emailSettings);
    }

    private void AddRepositories(IServiceCollection services)
    {
        services.AddSingleton<IPartnerRepository, PartnerRepository>();
        services.AddSingleton<IDbAccessRepository, DbAccessRepository>();
    }

    private void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IBetDaqService, BetDaqService>();
        services.AddSingleton<IBetRadarService, BetRadarService>();
        services.AddSingleton<IBetTypeService, BetTypeService>();
        services.AddSingleton<IEventFeedService, EventFeedService>();
        services.AddSingleton<IFeedCachingOperator, FeedCachingOperator>();
        services.AddSingleton<IFeedsService, FeedsService>();
        services.AddSingleton<IOddsFeedService, OddsFeedService>();
        services.AddSingleton<IPartnerService, PartnerService>();
        services.AddSingleton<IRateLimitService, RateLimitService>();
        services.AddSingleton<IResultFeedService, ResultFeedService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IVersionCachingService, VersionCachingService>();
    }

    private static void ConfigDataProtection(IServiceCollection services, IConfiguration configuration)
    {
        services.AddRedisCache(configuration.GetSection("RedisConfig"));

        var redis = ConnectionMultiplexer.Connect(configuration.GetValue<string>("RedisConfig:ConnectionString"));
        string appName = configuration.GetValue<string>("RedisConfig:ApplicationName");
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
            ctx.Request.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
            {
                Public = true,
                MaxAge = TimeSpan.FromSeconds(60)
            };
            await next();
        });

        var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
        string feedUrlPath = configuration.GetValue<string>("AppSettings:FeedUrlPath");
        if (!string.IsNullOrWhiteSpace(feedUrlPath)) feedUrlPath = "/";

        app.RegisterXMLOddsfeedRoutes(feedUrlPath);
    }
}