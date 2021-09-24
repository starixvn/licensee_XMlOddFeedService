namespace Fanex.OddsFeed.WebService.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.OddsFeed.Common.DTO;
    using Fanex.OddsFeed.Common.Repository;
    using Fanex.OddsFeed.WebServiceLibrary.Helpers;
    using WebServiceLibrary.Services;

    public class BetTypeController : BaseController
    {
        private const int TimeExpireCacheForBetType = 60;
        private readonly IPartnerService _partnerService;

        public BetTypeController()
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
                    : XmlContent(GetBetTypes(username, authenticatedPartner.UserProfile));
        }

        protected string GetBetTypes(string userName, UserProfile userProfile)
        {
            var domainName = RequestHelper.DomainName(Request.ServerVariables);
            var ipAddress = GetClientIpAddress();
            var cacheKeyBetType = "BetType_" + userName + ipAddress + domainName;
            var betTypes = RedisRepository.Get<string>(cacheKeyBetType);
            if (betTypes == null)
            {
                betTypes = _partnerService.GetBetTypesXmlContent(userName, userProfile);

                RedisRepository.Set(cacheKeyBetType, betTypes, TimeSpan.FromSeconds(TimeExpireCacheForBetType));
            }

            return betTypes;
        }
    }
}