namespace Fanex.OddsFeed.WebService.Controllers
{
    using System.Web.Mvc;
    using Fanex.OddsFeed.Common.DTO;
    using WebServiceLibrary.Services;

    public class BetTypeController : BaseController
    {
        private const int TimeExpireCacheForBetType = 277;
        private readonly IPartnerService _partnerService;
        private readonly NLog.Logger fileLogger = NLog.LogManager.GetCurrentClassLogger();

        public BetTypeController()
        {
            _partnerService = new PartnerService();
        }

        [OutputCache(Duration = TimeExpireCacheForBetType, VaryByParam = "username", VaryByCustom = "clientIpAddress")]
        public ActionResult Index(string username)
        {
            var ipAddress = GetClientIpAddress();
            fileLogger.Info(string.Format("BetType-{0}-{1}", username, ipAddress));

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
            var betTypes = _partnerService.GetBetTypesXmlContent(userName, userProfile);
            return betTypes;
        }
    }
}