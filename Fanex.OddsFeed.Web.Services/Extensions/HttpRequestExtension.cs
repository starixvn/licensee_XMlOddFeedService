namespace Fanex.OddsFeed.Web.Services.Extensions
{
    using Microsoft.AspNetCore.Http;

    public static class HttpRequestExtension
    {
        public static string GetValueFromRequestByKey(this HttpRequest request, string key)
        {
            string returnValue = string.Empty;
            var query = request.Query;
            var form = request.Form;
            var cookies = request.Cookies;

            if (!string.IsNullOrWhiteSpace(query[key].ToString()))
            {
                returnValue = query[key].ToString();
            }

            if (!string.IsNullOrWhiteSpace(form[key].ToString()))
            {
                returnValue = form[key].ToString();
            }

            if (!string.IsNullOrWhiteSpace(cookies[key].ToString()))
            {
                returnValue = cookies[key].ToString();
            }

            return returnValue;
        }
    }
}