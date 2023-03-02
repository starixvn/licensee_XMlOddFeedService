namespace Fanex.OddsFeed.Web.Controllers
{
    using System;
    using Fanex.OddsFeed.Core.Services;
    using Fanex.OddsFeed.Models.DTO;
    using Fanex.OddsFeed.Web.Services.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    public class BetTypeController : BaseController
    {
        private const int TimeExpireCacheForBetType = 277;
        private readonly IPartnerService partnerService;
        private readonly ILogServiceProvider logServiceProvider;

        public BetTypeController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            partnerService = serviceProvider.GetRequiredService<IPartnerService>();
            logServiceProvider = serviceProvider.GetRequiredService<ILogServiceProvider>();
        }

        [ResponseCache(Duration = TimeExpireCacheForBetType, VaryByQueryKeys = new[] { "username", "clientIpAddress" })]
        public async Task<IActionResult> Index(string username)
        {
            var ipAddress = GetClientIpAddress();
            logServiceProvider.LogFile(string.Format("BetType-{0}-{1}", username, ipAddress));

            if (await IsDownTimeAsync())
            {
                return XmlContent(await GetDowntimeContent());
            }

            var authenticatedPartner = await GetAuthenticatedPartner(username);

            return !string.IsNullOrEmpty(authenticatedPartner.Error)
                    ? XmlContent(authenticatedPartner.Error)
                    : XmlContent(await GetBetTypes(username, authenticatedPartner.UserProfile));
        }

        private async Task<string> GetBetTypes(string userName, UserProfile userProfile)
        {
            var betTypes = await partnerService.GetBetTypesXmlContent(userName, userProfile);
            return betTypes;
        }
    }
}