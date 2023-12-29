namespace OddsFeed.WebService.Extensions
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;

    public static class RequestHelper
    {
        public static string GetClientIpAddress(HttpContext httpContext)
        {
            var clientIp = httpContext.Request.Headers["X_FORWARDED_FOR"].ToString();
            if (string.IsNullOrEmpty(clientIp))
            {
                clientIp = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
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

        public static string DomainName(HttpContext httpContext) => $"{httpContext.Request.Scheme}://{httpContext.Request.Host.Value}";

        public static string FullDomainName(HttpContext httpContext)
        {
            var fulldomain = $"{httpContext.Request.Scheme}://{httpContext.Request.Host.Value}";
            fulldomain += httpContext.Request.PathBase.Value == "/" ? string.Empty : httpContext.Request.PathBase.Value;
            return fulldomain;
        }
    }
}