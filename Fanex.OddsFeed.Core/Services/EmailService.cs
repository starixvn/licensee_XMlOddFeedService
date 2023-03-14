namespace Fanex.OddsFeed.Core.Services
{
    using System.Net;
    using System.Net.Mail;
    using System.Text;
    using System.Text.RegularExpressions;
    using Fanex.OddsFeed.Core.Models.Email;
    using Microsoft.Extensions.Configuration;

    public interface IEmailService
    {
        void Send(EmailModel model);
    }

    public class EmailService : IEmailService
    {
        private readonly ILogServiceProvider logServiceProvider;
        private readonly IConfiguration configuration;

        public EmailService(ILogServiceProvider logServiceProvider, IConfiguration configuration)
        {
            this.logServiceProvider = logServiceProvider;
            this.configuration = configuration;
        }

        public void Send(EmailModel model)
        {
            EmailSettings emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();

            if (!IsValidEmail(model.SenderEmail) || !IsValidEmail(model.ReceiverEmail) || emailSettings.IsDisabledSendMail)
            {
                return;
            }

            try
            {
                var client = new SmtpClient(emailSettings.Host, emailSettings.Port);
                var msg = new MailMessage
                {
                    From = new MailAddress(emailSettings.Email, emailSettings.DisplayName)
                };
                GetMailFromString(model.ReceiverEmail, msg.To);
                GetMailFromString(model.MailCC, msg.CC);
                msg.Subject = model.Subject;
                msg.SubjectEncoding = Encoding.UTF8;
                msg.IsBodyHtml = true;
                msg.Body = model.Body;
                msg.BodyEncoding = Encoding.UTF8;
                msg.Priority = MailPriority.Normal;

                ServicePointManager.ServerCertificateValidationCallback += (_, _, _, _) => true;

                client.SendAsync(msg, null);

                logServiceProvider.LogFile(string.Format("To: {0} - CC: {1} - Subject: {2}", msg.To, msg.CC, model.Subject));
            }
            catch (Exception ex)
            {
                logServiceProvider.LogFile("==========================Email error==============================");
                logServiceProvider.LogFile(ex.Message);
            }
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return regex.IsMatch(email);
        }

        private static void GetMailFromString(string mailString, MailAddressCollection mails)
        {
            if (!string.IsNullOrWhiteSpace(mailString))
            {
                var emails = RemoveDoubleComma(mailString);
                var mailArrs = emails.Split(';');
                foreach (var mail in mailArrs)
                {
                    if (!string.IsNullOrEmpty(mail))
                    {
                        mails.Add(mail);
                    }
                }
            }
        }

        private static string RemoveDoubleComma(string mails)
        {
            var emails = mails;
            while (emails.Contains(",,"))
            {
                emails = emails.Replace(",,", ",");
            }

            while (emails.Contains(";;"))
            {
                emails = emails.Replace(";;", ";");
            }

            return emails.TrimEnd(new[] { ',', ';' });
        }
    }
}