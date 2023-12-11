namespace OddsFeed.WebService.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using OddsFeed.WebService.Builders;
    using OddsFeed.WebService.Extensions;
    using OddsFeed.WebService.Services.Interfaces;

    public class EventController : BaseController
    {
        private const int TimeExpireCacheForEvent = 60;
        private readonly IEventFeedService eventFeedService;

        public EventController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            eventFeedService = serviceProvider.GetRequiredService<IEventFeedService>();
        }

        [ResponseCache(Duration = TimeExpireCacheForEvent, VaryByQueryKeys = new[] { "username", "date", "clientIpAddress" })]
        public async Task<IActionResult> Index(string username)
        {
            var domain = RequestHelper.DomainName(HttpContext);
            var data = await GetEventFeed(username, domain);

            return XmlContent(data);
        }

        private async Task<string> GetEventFeed(string username, string domain, bool isSample = false, bool outrights = true)
        {
            if (string.IsNullOrEmpty(username))
            {
                return ErrorBuilder.GetParameterSufficent();
            }

            var authenticatedPartner = await GetAuthenticatedPartner(username, domain, GetClientIpAddress());

            if (string.IsNullOrEmpty(authenticatedPartner.Error) || isSample)
            {
                return await eventFeedService.BuildXmlEvent(authenticatedPartner.UserProfile, isSample, outrights);
            }

            return authenticatedPartner.Error;
        }
    }
}