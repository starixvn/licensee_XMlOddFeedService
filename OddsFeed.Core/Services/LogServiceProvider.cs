namespace OddsFeed.Core.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Logging.Extensions;
    using Microsoft.Extensions.Logging;

    public class LogServiceProvider(
        Fanex.Logging.ILogger logger,
        ILogger<LogServiceProvider> logfile) : ILogServiceProvider
    {
        public async Task LogInfoSentryAsync(string message)
        {
            await logger.InfoAsync(message);
        }

        public async Task LogSentryAsync(string message, Exception exception)
        {
            await logger.ErrorAsync(message, exception);
        }

        public async Task LogSentryAsync(Exception exception)
        {
            var logMessage = new LogMessage(exception.Message)
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