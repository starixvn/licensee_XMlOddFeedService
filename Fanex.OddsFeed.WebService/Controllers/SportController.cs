namespace Fanex.OddsFeed.WebService.Controllers
{
    using System.Web.Mvc;
    using Fanex.OddsFeed.Common.DTO;
    using WebServiceLibrary.Services;

    public class SportController : BaseController
    {
        private const int TimeExpireCacheForSport = 277;
        private readonly IPartnerService _partnerService;
        private readonly NLog.Logger fileLogger = NLog.LogManager.GetCurrentClassLogger();
        public SportController()
        {
            _partnerService = new PartnerService();
        }

        [OutputCache(Duration = TimeExpireCacheForSport, VaryByParam = "username", VaryByCustom = "clientIpAddress")]
        public ActionResult Index(string username)
        {
            var ipAddress = GetClientIpAddress();
            fileLogger.Info(string.Format("Sport-{0}-{1}", username, ipAddress));

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
            var sports = _partnerService.GetSportsXmlContent(userName, userProfile);
            return sports;
        }
    }
}