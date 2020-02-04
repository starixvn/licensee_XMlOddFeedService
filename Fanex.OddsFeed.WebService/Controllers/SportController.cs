namespace Fanex.OddsFeed.WebService.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.Caching;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.WebServiceLibrary.Helpers;
    using WebServiceLibrary.Services;

    public class SportController : BaseController
    {
        private const int TimeExpireCacheForSport = 20;

        private readonly IPartnerService _partnerService;

        public SportController()
        {
            _partnerService = new PartnerService();
        }

        public ActionResult Index(string username)
        {
            var authenticatedPartner = GetAuthenticatedPartner(username);

            return !string.IsNullOrEmpty(authenticatedPartner.Error)
                    ? XmlContent(authenticatedPartner.Error)
                    : XmlContent(GetSports(username, authenticatedPartner.UserProfile));
        }

        protected string GetSports(string userName, UserProfile userProfile)
        {
            var domainName = RequestHelper.DomainName(Request.ServerVariables);
            var ipAddress = GetClientIpAddress();
            var cacheKeySport = "Sport_" + userName + ipAddress + domainName;
            var sports = ConfiguredCacheService.Get<string>(cacheKeySport);
            if (sports == null)
            {
                sports = _partnerService.GetSportsXmlContent(userName, userProfile);

                ConfiguredCacheService.Set(cacheKeySport, sports,
                        new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(TimeExpireCacheForSport)));
            }

            return sports;
        }
    }
}