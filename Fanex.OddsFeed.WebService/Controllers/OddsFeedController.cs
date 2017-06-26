namespace Fanex.OddsFeed.WebService.Controllers
{
    using System;
    using System.Text;
    using System.Web.Mvc;
    using OddsFeed.Common.Configurations;
    using OddsFeed.Common.DTO;
    using WebServiceLibrary;
    using WebServiceLibrary.Helpers;
    using WebServiceLibrary.Models;
    using WebServiceLibrary.Services;

    public class OddsFeedController : BaseController
    {
        private readonly IBetTypeService _betTypeService;
        private readonly IOddsFeedService _oddsFeedService;
        private readonly IUserService _userService;

        public OddsFeedController()
        {
            _betTypeService = new BetTypeService();
            _oddsFeedService = new OddsFeedService();
            _userService = new UserService();
        }

        [OutputCache(Duration = 5, Location = System.Web.UI.OutputCacheLocation.Server, VaryByParam = "username")]
        public ActionResult Index(string username)
        {
            var userQuery = RequestHelper.GetFilter(Request);

            if (RequestHelper.IsOverLimitedOptions(userQuery))
            {
                return XmlContent(ErrorBuilder.GetOverLimitedOptions());
            }

            var domainName = RequestHelper.DomainName(Request.ServerVariables);
            var ipAddress = GetClientIpAddress();
            var authenticatedPartner = GetAuthenticatedPartner(username, domainName, ipAddress);

            return !string.IsNullOrEmpty(authenticatedPartner.Error)
                ? XmlContent(authenticatedPartner.Error)
                : XmlContent(GetOddsFeed(new OddsFeedsModel
                {
                    BookieName = authenticatedPartner.SystemName,
                    DomainName = domainName,
                    UserProfile = authenticatedPartner.UserProfile,
                    UserQuery = userQuery,
                    IpAddress = ipAddress,
                    TimeStamp = Convert.ToInt32(HttpContext.Request["t"])
                }));
        }

        public ActionResult OldHelp(string username)
        {
            if (IsDownTime())
            {
                return Content(GetDowntimeContent());
            }

            if (string.IsNullOrEmpty(username))
            {
                return Content(ErrorBuilder.GetParameterSufficent());
            }

            var domainName = RequestHelper.DomainName(Request.ServerVariables);
            var ipAddress = GetClientIpAddress();
            var authenticatedPartner = GetAuthenticatedPartner(username, domainName, ipAddress);

            if (!string.IsNullOrEmpty(authenticatedPartner.Error))
            {
                return Content(authenticatedPartner.Error);
            }

            var sampleOdds = GetOddsFeed(new OddsFeedsModel
            {
                BookieName = authenticatedPartner.SystemName,
                DomainName = domainName,
                UserProfile = authenticatedPartner.UserProfile,
                UserQuery = new UserQuery(),
                IpAddress = ipAddress,
                TimeStamp = Convert.ToInt32(HttpContext.Request["t"]),
                IsSample = true
            });

            ViewBag.UserName = username;
            ViewBag.FullDomainName = RequestHelper.FullDomainName(Request.ServerVariables);
            ViewBag.VirtualPath = GetVirtualPath();
            ViewBag.Odds = GetFormatSampleOdds(sampleOdds);
            ViewBag.Sports = _userService.GetSports(username);
            ViewBag.Bettypes = _userService.GetBetTypes(username);

            if (string.IsNullOrEmpty(authenticatedPartner.Error))
            {
                return View("Help", authenticatedPartner.UserProfile);
            }
            else
            {
                return Content(sampleOdds);
            }
        }

        public ActionResult Help(string username)
        {
            if (IsDownTime())
            {
                return Content(GetDowntimeContent());
            }

            var domainName = RequestHelper.DomainName(Request.ServerVariables);
            var ipAddress = GetClientIpAddress();
            var authenticatedPartner = GetAuthenticatedPartner(username, domainName, ipAddress);

            if (!string.IsNullOrEmpty(authenticatedPartner.Error))
            {
                return Content(authenticatedPartner.Error);
            }

            var sampleOdds = GetOddsFeed(new OddsFeedsModel
            {
                BookieName = authenticatedPartner.SystemName,
                DomainName = domainName,
                UserProfile = authenticatedPartner.UserProfile,
                UserQuery = new UserQuery(),
                IpAddress = ipAddress,
                TimeStamp = Convert.ToInt32(HttpContext.Request["t"]),
                IsSample = true
            });

            ViewBag.Odds = GetFormatSampleOdds(sampleOdds);
            ViewBag.UserName = username;
            ViewBag.FullDomainName = RequestHelper.FullDomainName(Request.ServerVariables);
            ViewBag.VirtualPath = GetVirtualPath();
            ViewBag.ContactEmail = _userService.GetContactEmail(username);

            GenerateAppendixData(username);

            if (string.IsNullOrEmpty(authenticatedPartner.Error))
            {
                return View("NewHelp", authenticatedPartner.UserProfile);
            }
            else
            {
                return Content(sampleOdds);
            }
        }

        private static string GetFormatSampleOdds(string sampleOdds)
        {
            var provider = new StringBuilder(sampleOdds);
            provider.Replace("><", ">\n<");
            provider.Replace(" />", "></odd>");
            provider.Replace("<outrights>", "\t<outrights>")
                .Replace("<event", "\t\t<event")
                .Replace("<odd id", "\t\t\t<odd id")
                .Replace("</event>", "\t\t</event>")
                .Replace("</outrights>", "\t</outrights>")

                .Replace("<matches", "\t<matches")
                .Replace("<match id", "\t\t<match id")
                .Replace("<bookie", "\t\t\t<bookie")
                .Replace("<odd t", "\t\t\t\t<odd t")
                .Replace("</bookie", "\t\t\t</bookie")
                .Replace("</match>", "\t\t</match>")
                .Replace("</matches>", "\t</matches>");

            return provider.ToString();
        }

        private void GenerateAppendixData(string username)
        {
            var sports = _userService.GetSports(username);
            ViewBag.SportsCol1 = sports.ColumnOne;
            ViewBag.SportsCol2 = sports.ColumnTwo;
            ViewBag.SportsCol3 = sports.ColumnThree;

            var bettypes = _userService.GetBetTypes(username);
            ViewBag.BetTypesCol1 = bettypes.ColumnOne;
            ViewBag.BetTypesCol2 = bettypes.ColumnTwo;
            ViewBag.BetTypesCol3 = bettypes.ColumnThree;

            var countries = _userService.GetCountries(username);
            ViewBag.CountriesCol1 = countries.ColumnOne;
            ViewBag.CountriesCol2 = countries.ColumnTwo;
            ViewBag.CountriesCol3 = countries.ColumnThree;

            ViewBag.HighlightSportType = AppSettings.HighlightSportType.Split(',');
            ViewBag.HighlightBetType = AppSettings.HighlightBetType.Split(',');
            ViewBag.BetTypeList = _betTypeService.GetBetTypeList();
        }

        private string GetOddsFeed(OddsFeedsModel model)
        {
            return _oddsFeedService.BuildXmlDocument(model);
        }

        private static string GetVirtualPath()
        {
            var virtualPath = new StringBuilder(AppSettings.FeedUrlPath);

            if (!string.IsNullOrEmpty(AppSettings.FeedUrlPath))
            {
                virtualPath.Append("/");
            }

            virtualPath.Append(AppSettings.VirtualPathWebService);

            if (!string.IsNullOrEmpty(AppSettings.VirtualPathWebService))
            {
                virtualPath.Append("/");
            }

            return virtualPath.ToString();
        }
    }
}