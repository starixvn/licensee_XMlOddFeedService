namespace OddsFeed.WebService.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using OddsFeed.Core.Extensions;
    using OddsFeed.Core.Services;
    using OddsFeed.Models.OddsData;
    using OddsFeed.Models.Schemas;
    using OddsFeed.WebService.Builders;
    using OddsFeed.WebService.Extensions;
    using OddsFeed.WebService.Models;
    using OddsFeed.WebService.Services;

    public class OddsFeedController : BaseController
    {
        private const int TimeExpireCacheForOdds = 60;
        private readonly IBetTypeService betTypeService;
        private readonly IOddsFeedService oddsFeedService;
        private readonly IUserService userService;
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly ILogServiceProvider logServiceProvider;
        private readonly WebSettings webSettings;

        public OddsFeedController(IServiceProvider serviceProvider, WebSettings webSettings) : base(serviceProvider)
        {
            betTypeService = serviceProvider.GetRequiredService<IBetTypeService>();
            oddsFeedService = serviceProvider.GetRequiredService<IOddsFeedService>();
            userService = serviceProvider.GetRequiredService<IUserService>();
            hostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
            logServiceProvider = serviceProvider.GetRequiredService<ILogServiceProvider>();
            this.webSettings = webSettings;
        }

        #region Index

        [ResponseCache(Duration = TimeExpireCacheForOdds, VaryByQueryKeys = new[] { "username", "Sport", "Country", "Bettype", "League", "moretime", "clientIpAddress" })]
        public async Task<IActionResult> Index(string username)
        {
            try
            {
                var ipAddress = GetClientIpAddress();
                var protocol = HttpContext.Request.Headers["x-forwarded-proto"].ToString();
                var domainName = string.Format("{0}://{1}", !string.IsNullOrWhiteSpace(protocol) ? protocol : HttpContext.Request.Scheme, HttpContext.Request.Host.Value);
                logServiceProvider.LogInfoToFile(string.Format("Odds-{0}-{1}-{2}", username, domainName, ipAddress));
                if (await IsDownTimeAsync())
                {
                    return XmlContent(await GetDowntimeContent());
                }
                var userQuery = await GetFilter(HttpContext.Request);

                if (IsOverLimitedOptions(userQuery))
                {
                    return XmlContent(ErrorBuilder.GetOverLimitedOptions());
                }

                var authenticatedPartner = await GetAuthenticatedPartner(username);
                var timestamp = string.IsNullOrEmpty(HttpContext.Request.GetValueFromRequestByKey("t")) ? "0" : HttpContext.Request.GetValueFromRequestByKey("t");

                return !string.IsNullOrEmpty(authenticatedPartner.Error)
                    ? XmlContent(authenticatedPartner.Error)
                    : XmlContent(await GetOddsFeed(new OddsFeedsModel
                    {
                        BookieName = authenticatedPartner.SystemName,
                        UserProfile = authenticatedPartner.UserProfile,
                        UserQuery = userQuery,
                        Timestamp = Convert.ToInt32(timestamp),
                        Username = username,
                        IP = ipAddress
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
                await logServiceProvider.LogSentryAsync("OddsFeedController:Index: " + string.Join(Environment.NewLine, messages), ex);
                var exception = new Exception("OddsFeedController Error", ex);
                throw exception;
            }
        }

        private bool IsOverLimitedOptions(UserQuery userQuery)
        {
            var limitedOptionsNumber = webSettings.LimitedOptions;

            return userQuery.Sports.Count > limitedOptionsNumber ||
                userQuery.Countries.Count > limitedOptionsNumber ||
                userQuery.BetTypes.Count > limitedOptionsNumber ||
                userQuery.Leagues.Count > limitedOptionsNumber;
        }

        private async Task<string> GetOddsFeed(OddsFeedsModel model)
            => await oddsFeedService.BuildXmlDocumentAsync(model);

        private async Task<UserQuery> GetFilter(HttpRequest request)
        {
            var filterSport = request.GetValueFromRequestByKey("Sport").ToLower(CultureInfo.InvariantCulture);
            var filterCountry = request.GetValueFromRequestByKey("Country").ToLower(CultureInfo.InvariantCulture);
            var filterBettype = request.GetValueFromRequestByKey("Bettype").ToLower(CultureInfo.InvariantCulture);
            var filterLeague = request.GetValueFromRequestByKey("League").ToLower(CultureInfo.InvariantCulture);
            var moreTime = request.GetValueFromRequestByKey("moretime").ToLower(CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(filterCountry))
            {
                filterCountry = await StandardizeCountryCodeList(string.Join(",", ListConverter.ConvertToListString(filterCountry, true)).ToLower(CultureInfo.InvariantCulture));
                if (string.IsNullOrEmpty(filterCountry))
                {
                    filterCountry = "null";
                }
            }

            return new UserQuery
            {
                Sports = ListConverter.ConvertToListInt(filterSport),
                Countries = ListConverter.ConvertToListString(filterCountry, true),
                BetTypes = ListConverter.ConvertToListInt(filterBettype),
                Leagues = ListConverter.ConvertToListString(filterLeague, true),
                MoreTime = moreTime == "yes"
            };
        }

        private async Task<string> StandardizeCountryCodeList(string countryCodeList)
        {
            var countryCodes = countryCodeList.Split(',').ToList();
            var countries = await oddsFeedService.GetCountryListAsync();
            var filteredRows = countries.Select(x => countryCodes.Exists(y => y == x.CountryCode));
            return string.Join(",", filteredRows.Select(x => x).AsEnumerable());
        }

        #endregion Index

        #region Help

        public async Task<IActionResult> Help(string username)
        {
            if (await IsDownTimeAsync())
            {
                ViewBag.UmtTo = (await GetUndermaintenanceTimeAsync()).To;
                return View("UnderMaintainance");
            }

            var authenticatedPartner = await GetAuthenticatedPartner(username);

            if (!string.IsNullOrEmpty(authenticatedPartner.Error))
            {
                return Content(authenticatedPartner.Error);
            }

            ViewBag.UserName = username;
            ViewBag.VirtualPath = GetVirtualPath();
            ViewBag.ContactEmail = await userService.GetContactEmailAsync(username);

            await GenerateAppendixData(username);

            return View("Help", authenticatedPartner.UserProfile);
        }

        private async Task GenerateAppendixData(string username)
        {
            var sports = await userService.GetSportAsync(username);
            ViewBag.SportsCol1 = sports.ColumnOne;
            ViewBag.SportsCol2 = sports.ColumnTwo;
            ViewBag.SportsCol3 = sports.ColumnThree;

            var bettypes = await userService.GetBetTypeAsync(username);
            ViewBag.BetTypesCol1 = bettypes.ColumnOne;
            ViewBag.BetTypesCol2 = bettypes.ColumnTwo;
            ViewBag.BetTypesCol3 = bettypes.ColumnThree;

            var countries = await userService.GetCountryAsync(username);
            ViewBag.CountriesCol1 = countries.ColumnOne;
            ViewBag.CountriesCol2 = countries.ColumnTwo;
            ViewBag.CountriesCol3 = countries.ColumnThree;

            ViewBag.HighlightSportType = webSettings.HighlightSportType.Split(',');
            ViewBag.HighlightBetType = GetHighlightBetType();
            ViewBag.BetTypeList = await betTypeService.GetBetTypeListAsync();

            ViewBag.BetTypeDescriptions = ReadBetTypeDescriptionFromFile();
        }

        private IList<BetTypeDescription> ReadBetTypeDescriptionFromFile()
        {
            var filePath = Path.Combine(hostEnvironment.ContentRootPath, webSettings.BetTypeDescriptionPath);
            var values = ReadJsonFromFile.GetBetTypeSchemas<BetTypeDescription>(filePath);
            return values;
        }

        private string[] GetHighlightBetType()
        {
            var result = new List<string>();
            var items = webSettings.HighlightBetType.Split(',');
            foreach (var item in items)
            {
                if (item.Contains('-'))
                {
                    var itemArr = item.Split('-');
                    var start = int.Parse(itemArr[0]);
                    var end = int.Parse(itemArr[1]);
                    for (var i = start; i <= end; i++)
                    {
                        result.Add(i.ToString());
                    }
                }
                else
                {
                    result.Add(item);
                }
            }

            return result.ToArray();
        }

        private string GetVirtualPath()
        {
            var virtualPath = new StringBuilder(webSettings.FeedUrlPath);

            if (!string.IsNullOrWhiteSpace(webSettings.FeedUrlPath))
            {
                virtualPath.Append('/');
            }

            virtualPath.Append(webSettings.VirtualPathWebService);

            if (!string.IsNullOrWhiteSpace(webSettings.VirtualPathWebService))
            {
                virtualPath.Append('/');
            }

            return virtualPath.ToString();
        }

        #endregion Help
    }
}