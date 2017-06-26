namespace Fanex.OddsFeed.WebService.Controllers
{
    using System.Web.Mvc;
    using Fanex.OddsFeed.Common.DTO;
    using WebServiceLibrary.Attributes;
    using WebServiceLibrary.Services;

    public class SportController : BaseController
    {
        private readonly IPartnerService _partnerService;

        public SportController()
        {
            _partnerService = new PartnerService();
        }

        [PartnerPermission]
        public ActionResult Index(string username, UserProfile userProfile)
        {
            return XmlContent(_partnerService.GetSportsXmlContent(username, userProfile));
        }
    }
}