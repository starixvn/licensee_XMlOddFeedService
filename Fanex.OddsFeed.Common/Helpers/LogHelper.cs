namespace Fanex.OddsFeed.Common.Helpers
{
    using System;
    using Logging;

    public static class LogHelper
    {
        public static void Log(Exception exception)
        {
            var message = new[]
            {
                            "ERROR: " + exception.Message,
                            "SOURCE: " + exception.Source,
                            "TARGETSITE: " + exception.TargetSite,
                            "STACKTRACE: " + exception.StackTrace
                };
            LogService.Error(string.Join(Environment.NewLine, message), exception);
        }
    }
}