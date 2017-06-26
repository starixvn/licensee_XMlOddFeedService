namespace Fanex.OddsFeed.WebService.Controllers
{
    using System.Web.Mvc;
    using WebServiceLibrary;
    using WebServiceLibrary.Helpers;
    using WebServiceLibrary.Services;

    public class EventController : BaseController
    {
        public ActionResult Index(string username)
        {
            var domain = RequestHelper.DomainName(Request.ServerVariables);
            var data = GetEventFeed(username, domain);

            return XmlContent(data);
        }

        private string GetEventFeed(string username, string domain, bool isSample = false, bool outrights = true)
        {
            if (string.IsNullOrEmpty(username))
            {
                return ErrorBuilder.GetParameterSufficent();
            }

            var authenticatedPartner = GetAuthenticatedPartner(username, domain, GetClientIpAddress());

            if (string.IsNullOrEmpty(authenticatedPartner.Error) || isSample)
            {
                var oddsFeedService = new EventFeedService(authenticatedPartner.UserProfile);

                return oddsFeedService.BuildXmlEvent(isSample, outrights);
            }

            return authenticatedPartner.Error;
        }
    }
}