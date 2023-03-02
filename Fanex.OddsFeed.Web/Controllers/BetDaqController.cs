namespace Fanex.OddsFeed.Web.Controllers
{
    using System;
    using Fanex.OddsFeed.Web.Services.Extensions;
    using Fanex.OddsFeed.Web.Services.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class BetDaqController : BaseController
    {
        private readonly IBetDaqService betdaqService;

        public BetDaqController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            betdaqService = serviceProvider.GetRequiredService<IBetDaqService>();
        }

        public async Task<IActionResult> GetSchedule(string userName)
        {
            if (!userName.Equals("betdaq", StringComparison.CurrentCultureIgnoreCase))
            {
                return Content("You are not authorized to take this action.");
            }

            if (await IsDownTimeAsync())
            {
                return Content(string.Empty);
            }

            var scheduleContent = await GetScheduleContent(userName, RequestHelper.DomainName(HttpContext), HttpContext.Request.GetValueFromRequestByKey("s"));

            return Content(scheduleContent);
        }

        public async Task<IActionResult> GetOdds(string userName)
        {
            if (!userName.Equals("betdaq", StringComparison.CurrentCultureIgnoreCase))
            {
                return Content("You are not authorized to take this action.");
            }

            if (await IsDownTimeAsync())
            {
                return Content(string.Empty);
            }

            return Content(await GetOdds(userName, RequestHelper.DomainName(HttpContext), HttpContext.Request.GetValueFromRequestByKey("s")));
        }

        private async Task<string> GetScheduleContent(string username, string domainName, string filter)
        {
            if (string.IsNullOrEmpty(username))
            {
                return string.Empty;
            }

            var authenticatedPartner = await GetAuthenticatedPartner(username, domainName, GetClientIpAddress());

            if (string.IsNullOrEmpty(authenticatedPartner.Error))
            {
                return await betdaqService.GetSchedule(filter);
            }

            return string.Empty;
        }

        private async Task<string> GetOdds(string username, string domainName, string filter = null)
        {
            if (string.IsNullOrEmpty(username))
            {
                return string.Empty;
            }

            var authenticatedPartner = await GetAuthenticatedPartner(username, domainName, GetClientIpAddress());

            if (string.IsNullOrEmpty(authenticatedPartner.Error))
            {
                return await betdaqService.GetOddWithUserIsBetdaq(filter);
            }

            return string.Empty;
        }
    }
}