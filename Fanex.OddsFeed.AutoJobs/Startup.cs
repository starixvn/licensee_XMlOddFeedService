using Fanex.Caching.Redis;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.AspNetCore;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Sentry;
using Fanex.OddsFeed.AutoJobs.Models;
using Fanex.OddsFeed.AutoJobs.Services;
using Fanex.OddsFeed.AutoJobs.Services.Interfaces;
using Fanex.OddsFeed.AutoJobs.Workers;
using Fanex.OddsFeed.Core;
using Fanex.OddsFeed.Core.Configurations;
using Fanex.OddsFeed.Core.Extensions;
using Fanex.OddsFeed.Core.Repositories;
using Fanex.OddsFeed.Repository;
using Fanex.OddsFeed.Repository.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
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
        services.AddControllers();
        ConfigDataProtection(services, Configuration);
        CoreDI.RegistryCoreServices(services, Configuration);
        InjectRepositories(services);
        InjectServices(services, Configuration);
        AddConfiguration(services, Configuration);
        AddWorkers(services);
    }

    private void AddConfiguration(IServiceCollection services, IConfiguration configuration)
    {
        var appSettngs = configuration.GetSection("AppSettings").Get<AppSettings>();
        services.AddSingleton(appSettngs);

        var jobSettings = configuration.GetSection("JobSettings").Get<List<JobSetting>>();
        services.AddSingleton(jobSettings);
    }

    private void AddWorkers(IServiceCollection services)
    {
        services.AddHostedService<OddsfeedWorker>();
        services.AddHostedService<ResultFeedWorker>();
    }

    private void InjectServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IOddsDataService, OddsDataService>();
        services.AddSingleton<IOddsfeedService, OddsfeedService>();
        services.AddSingleton<IResultFeedService, ResultFeedService>();
        services.AddSingleton<IResultFeedDataService, ResultFeedDataService>();
    }

    private void InjectRepositories(IServiceCollection services)
    {
        services.AddSingleton<IDynamicRepository, DynamicRepository>();
        services.AddSingleton<IBaseRepository, BaseRepository>();

        services.AddSingleton<IDbAccessRepository, DbAccessRepository>();
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
            app.UseHsts();
        }
        app.UseStaticFiles();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Whoami}/{action=Index}/{id?}");
        });
    }
}