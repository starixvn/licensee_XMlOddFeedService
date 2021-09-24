namespace Fanex.OddsFeed.WebService.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.Common.Repository;
    using Fanex.OddsFeed.WebServiceLibrary.Helpers;
    using WebServiceLibrary.Services;

    public class SportController : BaseController
    {
        private const int TimeExpireCacheForSport = 60;

        private readonly IPartnerService _partnerService;

        public SportController()
        {
            _partnerService = new PartnerService();
        }

        public ActionResult Index(string username)
        {
            if (this.IsDownTime())
            {
                return XmlContent(GetDowntimeContent());
            }

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
            var sports = RedisRepository.Get<string>(cacheKeySport);
            if (sports == null)
            {
                sports = _partnerService.GetSportsXmlContent(userName, userProfile);

                RedisRepository.Set(cacheKeySport, sports, TimeSpan.FromSeconds(TimeExpireCacheForSport));
            }

            return sports;
        }
    }
}