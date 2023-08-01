namespace Fanex.OddsFeed.WebService.Controllers
{
    using System.Web.Mvc;
    using OddsFeed.Common.Configurations;
    using WebServiceLibrary.Helpers;
    using WebServiceLibrary.Services;

    public class FeedsController : BaseController
    {
        private readonly IFeedsService _feedsService;

        public FeedsController()
        {
            _feedsService = new FeedsService();
        }

        [HttpGet]
        public ActionResult Index(string username, string feedname, int? fromid)
        {
            var ipAddress = GetClientIpAddress();
            var domainName = RequestHelper.DomainName(Request.ServerVariables);
            var authenticatedPartner = GetAuthenticatedPartner(username, domainName, ipAddress);

            if (string.IsNullOrEmpty(authenticatedPartner.Error))
            {
                return XmlContent(authenticatedPartner.Error);
            }

            var feedsData = _feedsService.GetFeedsData(
                FormatType.Xml,
                feedname,
                username,
                domainName,
                ipAddress,
                fromid);

            return XmlContent(feedsData);
        }

        [HttpGet]
        public ActionResult GetJson(string username, string feedname, int? fromid)
        {
            var ipAddress = GetClientIpAddress();
            var domainName = RequestHelper.DomainName(Request.ServerVariables);
            var authenticatedPartner = GetAuthenticatedPartner(username, domainName, ipAddress);

            if (string.IsNullOrEmpty(authenticatedPartner.Error))
            {
                return Content(authenticatedPartner.Error);
            }

            var feedsData = _feedsService.GetFeedsData(
                FormatType.Json,
                feedname,
                username,
                domainName,
                ipAddress,
                fromid);

            return Content(feedsData);
        }
    }
}