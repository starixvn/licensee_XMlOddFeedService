namespace OddsFeed.Core.Services
{
    using System;
    using System.Threading.Tasks;

    public interface ILogServiceProvider
    {
        Task LogInfoSentryAsync(string message);

        Task LogSentryAsync(string message);

        Task LogSentryAsync(Exception exception);

        Task LogSentryAsync(string message, Exception exception);

        void LogInfoToFile(string message);

        void LogErrorToFile(Exception exception);
    }
}