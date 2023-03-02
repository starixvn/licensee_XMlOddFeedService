namespace Fanex.OddsFeed.Web.Controllers
{
    using System.Security;
    using System.Text;
    using Fanex.OddsFeed.Web.Services.Constants;
    using Microsoft.AspNetCore.Mvc;

    public class ErrorsController : Controller
    {
        public ActionResult Index()
        {
            var message = SecurityElement.Escape("Sorry for the inconvenience! We are not able to process your request this time. Please try again later");

            return Content(ErrorBuilder.GetException(message), "application/xml", Encoding.UTF8);
        }

        public ActionResult General()
        {
            return Content("General failure", "text/plain");
        }
    }
}