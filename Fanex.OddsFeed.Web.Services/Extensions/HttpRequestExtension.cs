namespace Fanex.OddsFeed.Web.Services.Extensions
{
    using Microsoft.AspNetCore.Http;

    public static class HttpRequestExtension
    {
        public static string GetValueFromRequestByKey(this HttpRequest request, string key)
        {
            string returnValue = string.Empty;
            var query = request.Query;

            if (!string.IsNullOrWhiteSpace(query[key].ToString()))
            {
                returnValue = query[key].ToString();
            }

            return returnValue;
        }
    }
}