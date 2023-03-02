namespace Fanex.OddsFeed.Web.Controllers
{
    using System;
    using Fanex.OddsFeed.Web.Services.Constants;
    using Fanex.OddsFeed.Web.Services.Extensions;
    using Fanex.OddsFeed.Web.Services.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class BetRadarController : BaseController
    {
        private readonly IBetRadarService betRadarService;

        public BetRadarController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            betRadarService = serviceProvider.GetRequiredService<IBetRadarService>();
        }

        public async Task<IActionResult> Index(string userName)
        {
            if (!userName.Equals("betradar", StringComparison.CurrentCultureIgnoreCase))
            {
                return XmlContent(ErrorBuilder.GetUnAuthenticated());
            }

            return await IsDownTimeAsync()
                ? XmlContent(await GetDowntimeContent())
                : XmlContent(await GetOddsFeed(userName));
        }

        private async Task<string> GetOddsFeed(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return ErrorBuilder.GetParameterSufficent();
            }

            var authenticatedPartner = await GetAuthenticatedPartner(username, RequestHelper.DomainName(HttpContext), GetClientIpAddress());

            return string.IsNullOrEmpty(authenticatedPartner.Error)
                ? await betRadarService.GetXmlDocumentAsync()
                : authenticatedPartner.Error;
        }
    }
}