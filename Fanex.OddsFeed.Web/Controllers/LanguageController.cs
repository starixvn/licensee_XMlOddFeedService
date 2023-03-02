namespace Fanex.OddsFeed.Web.Controllers
{
    using System;
    using Fanex.OddsFeed.Core.Services;
    using Fanex.OddsFeed.Models.DTO;
    using Fanex.OddsFeed.Web.Services.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class LanguageController : BaseController
    {
        private const int TimeExpireCacheForLeague = 277;
        private readonly IPartnerService partnerService;
        private readonly ILogServiceProvider logServiceProvider;

        public LanguageController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            partnerService = serviceProvider.GetRequiredService<IPartnerService>();
            logServiceProvider = serviceProvider.GetRequiredService<ILogServiceProvider>();
        }

        [ResponseCache(Duration = TimeExpireCacheForLeague, VaryByQueryKeys = new[] { "username", "date", "clientIpAddress" })]
        public async Task<IActionResult> Index(string username)
        {
            var ipAddress = GetClientIpAddress();
            logServiceProvider.LogFile(string.Format("League-{0}-{1}", username, ipAddress));
            if (await IsDownTimeAsync())
            {
                return XmlContent(await GetDowntimeContent());
            }

            var authenticatedPartner = await GetAuthenticatedPartner(username);

            return !string.IsNullOrEmpty(authenticatedPartner.Error)
                    ? XmlContent(authenticatedPartner.Error)
                    : XmlContent(await GetLeagues(username, authenticatedPartner.UserProfile));
        }

        private async Task<string> GetLeagues(string userName, UserProfile userProfile)
        {
            var leagues = await partnerService.GetLeaguesXmlContent(userName, userProfile);
            return leagues;
        }
    }
}