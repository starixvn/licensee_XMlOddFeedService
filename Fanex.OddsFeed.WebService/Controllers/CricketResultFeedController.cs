namespace Fanex.OddsFeed.WebService.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.Logging;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.WebServiceLibrary.Attributes;
    using Fanex.OddsFeed.WebServiceLibrary.Models;
    using Fanex.OddsFeed.WebServiceLibrary.Services;

    [Compress]
    public class CricketResultFeedController : BaseController
    {
        private const int TimeExpireCacheForOdds = 5;
        private readonly IResultFeedService resultFeedService;

        public CricketResultFeedController()
        {
            resultFeedService = new ResultFeedService();
        }

        [OutputCache(Duration = TimeExpireCacheForOdds, VaryByParam = "username", VaryByCustom = "clientIpAddress")]
        public ActionResult Index(string username)
        {
            try
            {
                if (IsDownTime())
                {
                    return XmlContent(GetDowntimeContent());
                }

                DateTime? date = null;
                if(!string.IsNullOrEmpty(Request["date"]))
                {
                    DateTime requestDate;
                    DateTime.TryParse(Request["date"], out requestDate);
                    date = requestDate;
                }

                const short MinDays = -5;
                if (date.HasValue && (date.Value.Date < DateTime.Today.AddDays(MinDays) || date.Value.Date > DateTime.Today))
                {
                    return XmlContent($"<Error id=\"10025\" message=\"You can only view historical data up to 5 days from today. You can only view 1 day's data at a time.\" />");
                }

                if (date.HasValue && date.Value.Date == DateTime.Today)
                {
                    date = null;
                }

                const byte Cricket = 50;
                var timestamp = string.IsNullOrEmpty(Request["t"]) ? "0" : Request["t"];
                var authenticatedPartner = GetAuthenticatedPartner(username);

                return !string.IsNullOrEmpty(authenticatedPartner.Error)
                    ? XmlContent(authenticatedPartner.Error)
                    : XmlContent(resultFeedService.BuildXmlDocument(new OddsFeedsModel
                    {
                        BookieName = authenticatedPartner.SystemName,
                        UserProfile = authenticatedPartner.UserProfile,
                        UserQuery = new UserQuery
                        {
                            Sports = new List<int> { Cricket }
                        },
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
                    "username: " +  Server.HtmlEncode(username),
                    "timestamp: " + Server.HtmlEncode(HttpContext.Request["t"])
                };
                LogService.Error("CricketResultFeedController:Index: " + string.Join(Environment.NewLine, messages), ex);
                var exception = new Exception("CricketResultFeedController Error", ex);
                throw exception;
            }
        }
    }
}