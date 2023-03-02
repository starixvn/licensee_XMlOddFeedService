namespace Fanex.OddsFeed.Web.Controllers
{
    using Fanex.OddsFeed.Core.Models.Email;
    using Fanex.OddsFeed.Core.Services;
    using Microsoft.AspNetCore.Mvc;

    public class AnalyticsController : Controller
    {
        private const string subjectMail = "Odds feed services: Email sending testing";
        private readonly IEmailService emailService;
        private readonly EmailSettings emailSettings;

        public AnalyticsController(IEmailService emailService, EmailSettings emailSettings)
        {
            this.emailService = emailService;
            this.emailSettings = emailSettings;
        }

        public IActionResult Email()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Email(string emailTo, string content)
        {
            emailService.Send(new EmailModel
            {
                SenderEmail = emailSettings.Email,
                ReceiverEmail = emailTo,
                Subject = subjectMail,
                Body = content
            });

            return View();
        }
    }
}