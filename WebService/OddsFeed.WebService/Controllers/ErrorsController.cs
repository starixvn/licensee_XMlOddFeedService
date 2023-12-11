namespace OddsFeed.WebService.Controllers
{
    using System.Security;
    using System.Text;
    using Microsoft.AspNetCore.Mvc;
    using OddsFeed.WebService.Builders;

    public class ErrorsController : Controller
    {
        public IActionResult Index()
        {
            var message = SecurityElement.Escape("Sorry for the inconvenience! We are not able to process your request this time. Please try again later");

            return Content(ErrorBuilder.GetException(message), "application/xml", Encoding.UTF8);
        }

        public IActionResult General()
        {
            return Content("General failure", "text/plain");
        }

        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}