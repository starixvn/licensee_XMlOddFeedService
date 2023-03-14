namespace Fanex.OddsFeed.Web.Filters
{
    using Fanex.OddsFeed.Core.Services;
    using Fanex.OddsFeed.Web.Services.Models;
    using Fanex.OddsFeed.Web.Services.Services.Interfaces;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class)]
    public class RateLimitFilterAttribute : ActionFilterAttribute
    {
        private const int TooManyRequestStatusCode = 429;
        private const string TooManyRequestDescription = "Too many request";
        private const string TooManyRequestMessage = "You have been blocked because of sending too many requests";
        private readonly WebSettings webSettings;
        private readonly IRateLimitService rateLimitService;
        private readonly ILogServiceProvider logServiceProvider;

        public RateLimitFilterAttribute(WebSettings webSettings, IRateLimitService rateLimitService, ILogServiceProvider logServiceProvider)
        {
            this.webSettings = webSettings;
            this.rateLimitService = rateLimitService;
            this.logServiceProvider = logServiceProvider;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            var ipAdress = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty;
            var isBlocked = rateLimitService.IsBlocked(ipAdress);

            if (httpContext is not null && (isBlocked || IsThrottled(ipAdress)))
            {
                var statusCodePagesFeature = httpContext?.Features.Get<IStatusCodePagesFeature>();

                httpContext.Response.StatusCode = TooManyRequestStatusCode;
                httpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = TooManyRequestDescription;
                if (statusCodePagesFeature is not null)
                {
                    statusCodePagesFeature.Enabled = true;
                }

                filterContext.Result = new ContentResult
                {
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
                throttleInfo.ExpiredAt = curTime.AddSeconds(webSettings.Throttler.TimeOutInSeconds);
                rateLimitService.UpdateThrottleInfo(ipAdress, throttleInfo);
            }
            else
            {
                if (throttleInfo.RequestCount >= webSettings.Throttler.RequestLimit)
                {
                    rateLimitService.AddToBlockList(ipAdress, curTime.AddSeconds(webSettings.Throttler.BlockTimeInSeconds));
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

            logServiceProvider.LogFile(logMessage);
        }
    }
}