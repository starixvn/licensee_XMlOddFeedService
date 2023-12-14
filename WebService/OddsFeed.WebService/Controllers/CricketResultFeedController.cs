namespace OddsFeed.WebService.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.Logging;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using OddsFeed.Models.OddsData;
    using OddsFeed.WebService.Extensions;
    using OddsFeed.WebService.Services.Interfaces;

    public class CricketResultFeedController : BaseController
    {
        private const int TimeExpireCacheForOdds = 5;
        private readonly IResultFeedService resultFeedService;

        public CricketResultFeedController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            resultFeedService = serviceProvider.GetRequiredService<IResultFeedService>();
        }

        [ResponseCache(Duration = TimeExpireCacheForOdds, VaryByQueryKeys = new[] { "username", "date", "clientIpAddress" })]
        public async Task<IActionResult> Index(string username)
        {
            try
            {
                if (await IsDownTimeAsync())
                {
                    return XmlContent(await GetDowntimeContent());
                }

                DateTime? date = null;
                if (!string.IsNullOrEmpty(HttpContext.Request.GetValueFromRequestByKey("date")))
                {
                    DateTime.TryParse(HttpContext.Request.GetValueFromRequestByKey("date"), out DateTime requestDate);
                    date = requestDate;
                }

                const short MinDays = -5;
                var today = DateTimeOffset.Now.Date;
                if (date.HasValue && (date.Value.Date < today.AddDays(MinDays) || date.Value.Date > today))
                {
                    return XmlContent($"<Error id=\"10025\" message=\"You can only view historical data up to 5 days from today. You can only view 1 day's data at a time.\" />");
                }

                if (date.HasValue && date.Value.Date == today)
                {
                    date = null;
                }

                const byte Cricket = 50;
                var timestamp = string.IsNullOrEmpty(HttpContext.Request.GetValueFromRequestByKey("t")) ? "0" : Request.GetValueFromRequestByKey("t");
                var authenticatedPartner = await GetAuthenticatedPartner(username);

                return !string.IsNullOrEmpty(authenticatedPartner.Error)
                    ? XmlContent(authenticatedPartner.Error)
                    : XmlContent(await resultFeedService.BuildXmlDocument(
                        new OddsFeedsModel
                        {
                            BookieName = authenticatedPartner.SystemName,
                            UserProfile = authenticatedPartner.UserProfile,
                            UserQuery = new UserQuery { Sports = new List<int> { Cricket } },
                            Timestamp = Convert.ToInt32(timestamp),
                            Username = username,
                            Date = date
                        }));
            }
            catch (Exception ex)
            {
                var messages = new[]
                {
                    "Message: " +  ex.Message,
                    "username: " +  HttpUtility.HtmlEncode(username),
                    "timestamp: " + HttpUtility.HtmlEncode(HttpContext.Request.GetValueFromRequestByKey("t"))
                };
                LogService.Error("CricketResultFeedController:Index: " + string.Join(Environment.NewLine, messages), ex);
                var exception = new Exception("CricketResultFeedController Error", ex);
                throw exception;
            }
        }
    }
}