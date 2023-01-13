namespace Fanex.OddsFeed.Core.Services
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using NLog;
    using NLog.Web;

    public interface ILogServiceProvider
    {
        Task LogSentryAsync(Exception exception);

        Task LogSentryAsync(string message);

        void LogInfo(string message);

        void LogInfo(Exception exception);
    }

    public class LogServiceProvider : ILogServiceProvider
    {
        private readonly Logging.ILogger logger;
        private readonly Logger nlog = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        public LogServiceProvider(
            Logging.ILogger logger)
        {
            this.logger = logger;
        }

        public async Task LogSentryAsync(Exception exception)
        {
            var logMessage = new LogMessage(exception.Message)
                       .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                       .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                       .ToString();
            await logger.ErrorAsync(logMessage);
        }

        public async Task LogSentryAsync(string message)
        {
            await logger.ErrorAsync(message);
        }

        public void LogInfo(string message)
        {
            nlog.Info(message);
        }

        public void LogInfo(Exception exception)
        {
            nlog.Error(exception);
        }
    }
}