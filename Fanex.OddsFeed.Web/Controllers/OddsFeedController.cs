namespace Fanex.OddsFeed.Web.Controllers
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Web;
    using Fanex.OddsFeed.Core.Extensions;
    using Fanex.OddsFeed.Core.Services;
    using Fanex.OddsFeed.Models.OddsData;
    using Fanex.OddsFeed.Models.Schemas;
    using Fanex.OddsFeed.Web.Services.Constants;
    using Fanex.OddsFeed.Web.Services.Extensions;
    using Fanex.OddsFeed.Web.Services.Models;
    using Fanex.OddsFeed.Web.Services.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class OddsFeedController : BaseController
    {
        private const int TimeExpireCacheForOdds = 60;
        private readonly IBetTypeService betTypeService;
        private readonly IOddsFeedService oddsFeedService;
        private readonly IUserService userService;
        private readonly IFeedsService feedsService;
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly ILogServiceProvider logServiceProvider;
        private readonly WebSettings webSettings;

        public OddsFeedController(IServiceProvider serviceProvider, WebSettings webSettings) : base(serviceProvider)
        {
            betTypeService = serviceProvider.GetRequiredService<IBetTypeService>();
            oddsFeedService = serviceProvider.GetRequiredService<IOddsFeedService>();
            userService = serviceProvider.GetRequiredService<IUserService>();
            feedsService = serviceProvider.GetRequiredService<IFeedsService>();
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
                logServiceProvider.LogFile(string.Format("Odds-{0}-{1}", username, ipAddress));
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
                var timestamp = string.IsNullOrEmpty(HttpContext.Request.GetValueFromRequestByKey("t")) ? "0" : HttpContext.Request.GetValueFromRequestByKey("t").ToString();

                return !string.IsNullOrEmpty(authenticatedPartner.Error)
                    ? XmlContent(authenticatedPartner.Error)
                    : XmlContent(await GetOddsFeed(new OddsFeedsModel
                    {
                        BookieName = authenticatedPartner.SystemName,
                        UserProfile = authenticatedPartner.UserProfile,
                        UserQuery = userQuery,
                        Timestamp = Convert.ToInt32(timestamp),
                        Username = username
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
            => await oddsFeedService.BuildXmlDocument(model);

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
            const int valueIndex = 2;

            var countryCodeTable = await oddsFeedService.GetCountryListAsync();
            var filteredRows = countryCodeTable.Select($"{"CountryCode"} in ('{countryCodeList.Replace(",", "','")}')");
            return string.Join(",", filteredRows.Select(x => x.ItemArray.GetValue(valueIndex)).AsEnumerable());
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
            ViewBag.FullDomainName = RequestHelper.FullDomainName(HttpContext);
            ViewBag.VirtualPath = GetVirtualPath();
            ViewBag.ContactEmail = await userService.GetContactEmail(username);

            await GenerateAppendixData(username);

            return View("Help", authenticatedPartner.UserProfile);
        }

        private async Task GenerateAppendixData(string username)
        {
            var sports = await userService.GetSports(username);
            ViewBag.SportsCol1 = sports.ColumnOne;
            ViewBag.SportsCol2 = sports.ColumnTwo;
            ViewBag.SportsCol3 = sports.ColumnThree;

            var bettypes = await userService.GetBetTypes(username);
            ViewBag.BetTypesCol1 = bettypes.ColumnOne;
            ViewBag.BetTypesCol2 = bettypes.ColumnTwo;
            ViewBag.BetTypesCol3 = bettypes.ColumnThree;

            var countries = await userService.GetCountries(username);
            ViewBag.CountriesCol1 = countries.ColumnOne;
            ViewBag.CountriesCol2 = countries.ColumnTwo;
            ViewBag.CountriesCol3 = countries.ColumnThree;

            ViewBag.HighlightSportType = webSettings.HighlightSportType.Split(',');
            ViewBag.HighlightBetType = GetHighlightBetType();
            ViewBag.BetTypeList = await betTypeService.GetBetTypeList();

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

            if (!string.IsNullOrEmpty(webSettings.FeedUrlPath))
            {
                virtualPath.Append('/');
            }

            virtualPath.Append(webSettings.VirtualPathWebService);

            if (!string.IsNullOrEmpty(webSettings.VirtualPathWebService))
            {
                virtualPath.Append('/');
            }

            return virtualPath.ToString();
        }

        #endregion Help
    }
}