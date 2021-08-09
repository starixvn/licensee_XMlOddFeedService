namespace Fanex.OddsFeed.WebService.App_Start
{
    using System;
    using System.Web.Mvc;
    using Fanex.OddsFeed.Common.Configurations;
    using Fanex.OddsFeed.WebServiceLibrary.Caching;
    using NLog;

    [AttributeUsage(AttributeTargets.Class)]
    public class RateLimitFilterAttribute : ActionFilterAttribute
    {
        private const int TooManyRequestStatusCode = 429;
        private const string TooManyRequestDescription = "Too many request";
        private const string TooManyRequestMessage = "You have been blocked because of sending too many requests";

        private readonly int RequestLimit = int.Parse(AppSettings.ThrottlerRequestLimit);
        private readonly int BlockTimeInSeconds = int.Parse(AppSettings.ThrollerBlockTimeInSeconds);
        private readonly int TimeOutInSeconds = int.Parse(AppSettings.ThrottlerTimeOutInSeconds);
        private readonly IRateLimitService rateLimitService;

        private readonly Logger fileLogger = LogManager.GetCurrentClassLogger();
        public RateLimitFilterAttribute()
        {
            rateLimitService = new RateLimitService();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            var ipAdress = httpContext.Request.UserHostAddress;
            var isBlocked = rateLimitService.IsBlocked(ipAdress);

            if (isBlocked || IsThrottled(ipAdress))
            {
                httpContext.Response.StatusCode = TooManyRequestStatusCode;
                httpContext.Response.StatusDescription = TooManyRequestDescription;
                httpContext.Response.TrySkipIisCustomErrors = true;

                filterContext.Result = new ContentResult {
                    Content = TooManyRequestMessage
                };
            }

            base.OnActionExecuting(filterContext);
        }

        private bool IsThrottled(string ipAdress)
        {
            bool result = false;
            var throttleInfo = rateLimitService.GetThrottleInfo(ipAdress);
            var curTime = DateTime.Now;

            if (throttleInfo.ExpiredAt <= curTime)
            {
                throttleInfo.RequestCount = 1;
                throttleInfo.ExpiredAt = curTime.AddSeconds(TimeOutInSeconds);
                rateLimitService.UpdateThrottleInfo(ipAdress, throttleInfo);
            }
            else
            {
                if (throttleInfo.RequestCount >= RequestLimit)
                {
                    rateLimitService.AddToBlockList(ipAdress, curTime.AddSeconds(BlockTimeInSeconds));
                    LogBlockInfo(ipAdress);

                    result = true;
                }
                else
                {
                    throttleInfo.RequestCount++;
                    rateLimitService.UpdateThrottleInfo(ipAdress, throttleInfo);
                }
            }

            return result;
        }

        private void LogBlockInfo(string ipAdress)
        {
            const string LogMessagePrefix = "TooManyRequests - IP Address: ";
            string logMessage = LogMessagePrefix + ipAdress;

            fileLogger.Warn(logMessage);
        }
    }
}