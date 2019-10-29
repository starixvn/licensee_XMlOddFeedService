namespace Fanex.OddsFeed.WebService.Controllers
{
    using System.Web.Mvc;
    using Common.Configurations;
    using WebServiceLibrary.Helpers;

    public class AnalyticsController : Controller
    {
        private const string subjectMail = "Odds feed services: Email sending testing";

        public ActionResult Email()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Email(string emailTo, string content)
        {
            ViewBag.Sent = MailHelper.SendEmail(AppSettings.FromMail, emailTo, subjectMail, content);

            return View();
        }
    }
}
