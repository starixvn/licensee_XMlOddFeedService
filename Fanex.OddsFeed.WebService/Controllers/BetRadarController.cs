namespace Fanex.OddsFeed.WebService.Controllers
{
    using System;
    using System.Web.Mvc;
    using WebServiceLibrary;
    using WebServiceLibrary.Helpers;
    using WebServiceLibrary.Services;

    public class BetRadarController : BaseController
    {
        private readonly IBetRadarService _betRadarService;

        public BetRadarController()
        {
            _betRadarService = new BetRadarService();
        }

        public ActionResult Index(string userName)
        {
            if (!userName.Equals("betradar", StringComparison.CurrentCultureIgnoreCase))
            {
                return XmlContent(ErrorBuilder.GetUnAuthenticated());
            }

            return IsDownTime()
                ? XmlContent(GetDowntimeContent())
                : XmlContent(GetOddsFeed(userName));
        }

        private string GetOddsFeed(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return ErrorBuilder.GetParameterSufficent();
            }

            var authenticatedPartner = GetAuthenticatedPartner(username, RequestHelper.DomainName(Request.ServerVariables), GetClientIpAddress());

            return string.IsNullOrEmpty(authenticatedPartner.Error)
                ? _betRadarService.GetXmlDocument()
                : authenticatedPartner.Error;
        }
    }
}
