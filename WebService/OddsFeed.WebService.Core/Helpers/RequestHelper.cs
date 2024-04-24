namespace OddsFeed.WebService.Extensions
{
    using System;
    using Microsoft.AspNetCore.Http;

    public static class RequestHelper
    {
        public static string GetClientIpAddress(HttpContext httpContext)
        {
            var clientIp = httpContext.Request.Headers["x-forwarded-for"].ToString();
            if (string.IsNullOrEmpty(clientIp))
            {
                clientIp = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
            else
            {
                var forwardedIps = clientIp.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                clientIp = forwardedIps[0];
            }

            if (string.Equals(clientIp, "0.0.0.1"))
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
            ////var protocol = httpContext.Request.Headers["x-forwarded-proto"].ToString();
            return string.Format("https://{0}", httpContext.Request.Host.Value);
        }
    }
}