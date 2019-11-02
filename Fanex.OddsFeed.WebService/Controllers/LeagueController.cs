namespace Fanex.OddsFeed.WebService.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.Caching;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.WebServiceLibrary.Helpers;
    using WebServiceLibrary.Services;

    public class LeagueController : BaseController
    {
        private const int TimeExpireCacheForLeague = 20;
        private readonly IPartnerService _partnerService;

        public LeagueController()
        {
            _partnerService = new PartnerService();
        }

        public ActionResult Index(string username)
        {
            var authenticatedPartner = GetAuthenticatedPartner(username);

            return !string.IsNullOrEmpty(authenticatedPartner.Error)
                    ? XmlContent(authenticatedPartner.Error)
                    : XmlContent(GetLeagues(username, authenticatedPartner.UserProfile));
        }

        protected string GetLeagues(string userName, UserProfile userProfile)
        {
            var domainName = RequestHelper.DomainName(Request.ServerVariables);
            var ipAddress = GetClientIpAddress();
            var cacheKeyLeague = "League_" + userName + ipAddress + domainName;
            var leagues = ConfiguredCacheService.Get<string>(cacheKeyLeague);
            if (leagues == null)
            {
                leagues = _partnerService.GetLeaguesXmlContent(userName, userProfile);

                ConfiguredCacheService.Set(cacheKeyLeague, leagues,
                        new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(TimeExpireCacheForLeague)));
            }

            return leagues;
        }
    }
}