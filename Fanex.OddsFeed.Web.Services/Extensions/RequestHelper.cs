using Microsoft.AspNetCore.Http;

namespace Fanex.OddsFeed.Web.Services.Extensions
{
    public static class RequestHelper
    {
        public static string GetClientIpAddress(HttpContext httpContext)
        {
            var clientIp = httpContext.Request?.Headers["X_FORWARDED_FOR"].ToString();

            if (string.IsNullOrEmpty(clientIp))
            {
                clientIp = httpContext?.Connection?.RemoteIpAddress?.ToString();
            }
            else
            {
                var forwardedIps = clientIp.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                clientIp = forwardedIps[forwardedIps.Length - 1];
            }

            if (string.Equals(clientIp, "::1"))
            {
                clientIp = "127.0.0.1";
            }

            return clientIp;
        }

        public static string DomainName(HttpContext httpContext, bool isDevelopment = true)
        {
            var protocol = httpContext.Request.IsHttps ? "https://" : "http://";
            var domain = httpContext.GetServerVariable("SERVER_NAME");
            var port = httpContext.GetServerVariable("SERVER_PORT");
            var fullDomainName = protocol + domain;

            if (isDevelopment)
            {
                fullDomainName = fullDomainName + ":" + port;
            }

            return fullDomainName;
        }

        public static string FullDomainName(HttpContext httpContext, bool isDevelopment = true)
        {
            var domain = httpContext.GetServerVariable("SERVER_NAME");
            var port = httpContext.GetServerVariable("SERVER_PORT");
            var protocol = httpContext.Request.IsHttps ? "https://" : "http://";
            var fulldomain = protocol + domain;

            if (isDevelopment)
            {
                fulldomain = fulldomain + ":" + port;
            }

            fulldomain += (httpContext.Request.PathBase.Value == "/" ? string.Empty : httpContext.Request.PathBase.Value);
            return fulldomain;
        }
    }
}