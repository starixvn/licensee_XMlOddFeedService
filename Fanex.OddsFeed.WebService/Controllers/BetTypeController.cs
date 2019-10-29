namespace Fanex.OddsFeed.WebService.Controllers
{
    using System.Web.Mvc;
    using System.Web.UI;
    using WebServiceLibrary.Services;

    public class BetTypeController : BaseController
    {
        private const int TimeExpireCacheForBetType = 20;
        private readonly IPartnerService _partnerService;

        public BetTypeController()
        {
            _partnerService = new PartnerService();
        }

        [OutputCache(Duration = TimeExpireCacheForBetType, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "username", VaryByCustom = "clientIP")]
        public ActionResult Index(string username)
        {
            var authenticatedPartner = GetAuthenticatedPartner(username);

            return !string.IsNullOrEmpty(authenticatedPartner.Error)
                    ? XmlContent(authenticatedPartner.Error)
                    : XmlContent(_partnerService.GetBetTypesXmlContent(username, authenticatedPartner.UserProfile));
        }
    }
}