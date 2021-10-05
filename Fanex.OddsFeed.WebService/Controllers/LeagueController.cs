namespace Fanex.OddsFeed.WebService.Controllers
{
    using System.Web.Mvc;
    using Fanex.OddsFeed.Common.DTO;
    using WebServiceLibrary.Services;

    public class LeagueController : BaseController
    {
        private const int TimeExpireCacheForLeague = 277;
        private readonly IPartnerService _partnerService;
        private readonly NLog.Logger fileLogger = NLog.LogManager.GetCurrentClassLogger();

        public LeagueController()
        {
            _partnerService = new PartnerService();
        }

        [OutputCache(Duration = TimeExpireCacheForLeague, VaryByParam = "username", VaryByCustom = "clientIpAddress")]
        public ActionResult Index(string username)
        {
            var ipAddress = GetClientIpAddress();
            fileLogger.Info(string.Format("League-{0}-{1}", username, ipAddress));
            if (this.IsDownTime())
            {
                return XmlContent(GetDowntimeContent());
            }

            var authenticatedPartner = GetAuthenticatedPartner(username);

            return !string.IsNullOrEmpty(authenticatedPartner.Error)
                    ? XmlContent(authenticatedPartner.Error)
                    : XmlContent(GetLeagues(username, authenticatedPartner.UserProfile));
        }

        protected string GetLeagues(string userName, UserProfile userProfile)
        {
            var leagues  = _partnerService.GetLeaguesXmlContent(userName, userProfile);
            return leagues;
        }
    }
}