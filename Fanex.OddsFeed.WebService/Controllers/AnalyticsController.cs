namespace Fanex.OddsFeed.WebService.Controllers
{
    using System.Text;
    using System.Web.Mvc;
    using Common.Configurations;
    using Common.DTO;
    using WebServiceLibrary.Caching;
    using WebServiceLibrary.Helpers;

    public class AnalyticsController : Controller
    {

        public ActionResult Email()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Email(string emailTo, string content)
        {
            var subject = "Odds feed services: Email sending testing";

            ViewBag.Sent = MailHelper.SendEmail(AppSettings.FromMail, emailTo, subject, content);

            return View();
        }
    }
}
