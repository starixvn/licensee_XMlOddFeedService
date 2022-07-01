namespace Fanex.OddsFeed.WebService.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.Logging;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.WebServiceLibrary.Attributes;
    using Fanex.OddsFeed.WebServiceLibrary.Models;
    using Fanex.OddsFeed.WebServiceLibrary.Services;

    [Compress]
    public class CricketResultFeedController : BaseController
    {
        private const int TimeExpireCacheForOdds = 60;
        private readonly IResultFeedService resultFeedService;
        private readonly NLog.Logger fileLogger = NLog.LogManager.GetCurrentClassLogger();

        public CricketResultFeedController()
        {
            resultFeedService = new ResultFeedService();
        }

        [OutputCache(Duration = TimeExpireCacheForOdds, VaryByParam = "username", VaryByCustom = "clientIpAddress")]
        public ActionResult Index(string username)
        {
            try
            {
                var ipAddress = GetClientIpAddress();
                fileLogger.Info(string.Format("CricketResultFeed-{0}-{1}", username, ipAddress));
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

                var timestamp = string.IsNullOrEmpty(Request["t"]) ? "0" : Request["t"];
                var authenticatedPartner = GetAuthenticatedPartner(username);

                return !string.IsNullOrEmpty(authenticatedPartner.Error)
                    ? XmlContent(authenticatedPartner.Error)
                    : XmlContent(resultFeedService.BuildXmlDocument(new OddsFeedsModel
                    {
                        BookieName = authenticatedPartner.SystemName,
                        UserProfile = authenticatedPartner.UserProfile,
                        UserQuery = new UserQuery(),
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