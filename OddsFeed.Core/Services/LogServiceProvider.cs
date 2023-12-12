namespace OddsFeed.Core.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Microsoft.Extensions.Logging;

    public class LogServiceProvider : ILogServiceProvider
    {
        private readonly Fanex.Logging.ILogger logger;
        private readonly ILogger logfile;

        public LogServiceProvider(
            Fanex.Logging.ILogger logger,
            ILogger<LogServiceProvider> logfile)
        {
            this.logger = logger;
            this.logfile = logfile;
        }

        public async Task LogSentryAsync(string message, Exception exception)
        {
            await logger.ErrorAsync(message, exception);
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

        public void LogInfoToFile(string message)
        {
            logfile.LogInformation(message);
        }

        public void LogErrorToFile(Exception exception)
        {
            logfile.LogError(exception, exception.Message);
        }
    }
}