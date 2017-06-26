namespace Fanex.OddsFeed.WebService.Controllers
{
    using System.Web.Mvc;
    using OddsFeed.Common.DTO;
    using WebServiceLibrary.Attributes;
    using WebServiceLibrary.Services;

    public class BetTypeController : BaseController
    {
        private readonly IPartnerService _partnerService;

        public BetTypeController()
        {
            _partnerService = new PartnerService();
        }

        [PartnerPermission]
        public ActionResult Index(string username, UserProfile userProfile)
        {
            return XmlContent(_partnerService.GetBetTypesXmlContent(username, userProfile));
        }
    }
}