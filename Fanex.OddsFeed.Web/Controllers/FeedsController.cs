namespace Fanex.OddsFeed.Web.Controllers
{
    using System;
    using Fanex.OddsFeed.Web.Services.Enum;
    using Fanex.OddsFeed.Web.Services.Extensions;
    using Fanex.OddsFeed.Web.Services.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class FeedsController : BaseController
    {
        private readonly IFeedsService feedsService;

        public FeedsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            feedsService = serviceProvider.GetRequiredService<IFeedsService>();
        }

        [HttpGet]
        public async Task<IActionResult> Index(string username, string feedname, int? fromid)
        {
            var ipAddress = GetClientIpAddress();
            var domainName = RequestHelper.DomainName(HttpContext);
            var authenticatedPartner = await GetAuthenticatedPartner(username, domainName, ipAddress);

            if (string.IsNullOrEmpty(authenticatedPartner.Error))
            {
                return XmlContent(authenticatedPartner.Error);
            }

            var feedsData = await feedsService.GetFeedsData(
                FormatType.Xml,
                feedname,
                username,
                domainName,
                ipAddress,
                fromid);

            return XmlContent(feedsData);
        }

        [HttpGet]
        public async Task<IActionResult> GetJson(string username, string feedname, int? fromid)
        {
            var ipAddress = GetClientIpAddress();
            var domainName = RequestHelper.DomainName(HttpContext);
            var authenticatedPartner = await GetAuthenticatedPartner(username, domainName, ipAddress);

            if (string.IsNullOrEmpty(authenticatedPartner.Error))
            {
                return Content(authenticatedPartner.Error);
            }

            var feedsData = await feedsService.GetFeedsData(
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