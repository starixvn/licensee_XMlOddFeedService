namespace Fanex.OddsFeed.WebService.Controllers
{
    using System.Web.Mvc;
    using System.Web.UI;
    using WebServiceLibrary.Services;

    public class SportController : BaseController
    {
        private readonly IPartnerService _partnerService;

        public SportController()
        {
            _partnerService = new PartnerService();
        }

        [OutputCache(Duration = 20, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "username", VaryByCustom = "clientIP")]
        public ActionResult Index(string username)
        {
            var authenticatedPartner = GetAuthenticatedPartner(username);

            return !string.IsNullOrEmpty(authenticatedPartner.Error)
                    ? XmlContent(authenticatedPartner.Error)
                    : XmlContent(_partnerService.GetSportsXmlContent(username, authenticatedPartner.UserProfile));
        }
    }
}