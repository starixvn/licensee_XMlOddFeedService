namespace OddsFeed.WebService.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using OddsFeed.Core.Services;
    using OddsFeed.Models.DTO;
    using OddsFeed.WebService.Services;

    public class LeagueController : BaseController
    {
        private const int TimeExpireCacheForLeague = 277;
        private readonly IPartnerService partnerService;
        private readonly ILogServiceProvider logServiceProvider;

        public LeagueController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            partnerService = serviceProvider.GetRequiredService<IPartnerService>();
            logServiceProvider = serviceProvider.GetRequiredService<ILogServiceProvider>();
        }

        [ResponseCache(Duration = TimeExpireCacheForLeague, VaryByQueryKeys = new[] { "username", "date", "clientIpAddress" })]
        public async Task<IActionResult> Index(string username)
        {
            var ipAddress = GetClientIpAddress();
            logServiceProvider.LogInfoToFile(string.Format("League-{0}-{1}", username, ipAddress));
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
            var leagues = await partnerService.GetLeaguesXmlContentAsync(userName, userProfile);
            return leagues;
        }
    }
}