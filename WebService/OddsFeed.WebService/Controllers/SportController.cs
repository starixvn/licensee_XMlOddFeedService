namespace OddsFeed.WebService.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using OddsFeed.Models.DTO;
    using OddsFeed.WebService.Services;

    public class SportController : BaseController
    {
        private const int TimeExpireCacheForSport = 277;
        private readonly IPartnerService partnerService;
        private readonly NLog.Logger fileLogger = NLog.LogManager.GetCurrentClassLogger();

        public SportController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            partnerService = serviceProvider.GetRequiredService<IPartnerService>();
        }

        [ResponseCache(Duration = TimeExpireCacheForSport, VaryByQueryKeys = new[] { "username", "clientIpAddress" })]
        public async Task<IActionResult> Index(string username)
        {
            var ipAddress = GetClientIpAddress();
            fileLogger.Info(string.Format("Sport-{0}-{1}", username, ipAddress));

            if (await IsDownTimeAsync())
            {
                return XmlContent(await GetDowntimeContent());
            }

            var authenticatedPartner = await GetAuthenticatedPartner(username);

            return !string.IsNullOrEmpty(authenticatedPartner.Error)
                    ? XmlContent(authenticatedPartner.Error)
                    : XmlContent(await GetSports(username, authenticatedPartner.UserProfile));
        }

        protected async Task<string> GetSports(string userName, UserProfile userProfile)
        {
            var sports = await partnerService.GetSportsXmlContentAsync(userName, userProfile);
            return sports;
        }
    }
}