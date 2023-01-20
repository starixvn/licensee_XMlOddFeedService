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
using Fanex.OddsFeed.Web.Services.Extensions;
using Microsoft.AspNetCore.DataProtection;
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
        DI.RegisterServices(services);
        AddConfiguration(services, Configuration);
    }

    private void AddConfiguration(IServiceCollection services, IConfiguration configuration)
    {
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
        app.UseHsts();
        app.UseMiddleware<ErrorHandler>();
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