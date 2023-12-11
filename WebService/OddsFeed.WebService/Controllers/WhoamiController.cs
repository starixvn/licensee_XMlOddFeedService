namespace OddsFeed.WebService.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using OddsFeed.Core.Extensions;

    public class WhoamiController : Controller
    {
        [Route("Whoami")]
        [Route("Whoami.aspx")]
        public IActionResult Index()
        {
            return Content(WhoamiHelper.GetContent(HttpContext));
        }
    }
}