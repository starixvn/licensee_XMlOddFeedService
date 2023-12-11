namespace OddsFeed.WebService.Extensions
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;

    public static class RequestHelper
    {
        public static string GetClientIpAddress(HttpContext httpContext)
        {
            var clientIp = httpContext.Request?.Headers["X_FORWARDED_FOR"].ToString();

            if (string.IsNullOrEmpty(clientIp))
            {
                var feature = httpContext.Features.Get<IHttpConnectionFeature>();
                clientIp = feature?.RemoteIpAddress?.ToString();
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

            if (clientIp.StartsWith("::ffff:"))
            {
                clientIp = clientIp.Replace("::ffff:", string.Empty);
            }

            return clientIp;
        }

        public static string DomainName(HttpContext httpContext)
        {
            var domain = httpContext.Request.Host.Value;
            var port = httpContext.Request.Host.Port;
            var fullDomainName = $"{httpContext.Request.Scheme}://{domain}";

            if (port is not null)
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