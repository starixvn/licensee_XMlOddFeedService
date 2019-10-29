namespace Fanex.OddsFeed.WebService.Controllers
{
    using System.Threading;
    using System.Web.Mvc;
    using System.Web.UI;
    using WebServiceLibrary.Services;

    public class LeagueController : BaseController
    {
        private const int TimeExpireCacheForLeague = 20;
        private readonly IPartnerService _partnerService;

        public LeagueController()
        {
            _partnerService = new PartnerService();
        }

        [OutputCache(Duration = TimeExpireCacheForLeague, Location = OutputCacheLocation.Server, VaryByParam = "username", VaryByCustom ="clientIP")]
        public ActionResult Index(string username)
        {
            var authenticatedPartner = GetAuthenticatedPartner(username);


            return !string.IsNullOrEmpty(authenticatedPartner.Error)
                    ? XmlContent(authenticatedPartner.Error)
                    : XmlContent(_partnerService.GetLeaguesXmlContent(username, authenticatedPartner.UserProfile));
        }
    }
}