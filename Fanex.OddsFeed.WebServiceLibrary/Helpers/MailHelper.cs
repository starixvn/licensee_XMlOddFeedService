namespace Fanex.OddsFeed.WebServiceLibrary.Helpers
{
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Text;
    using Common.Configurations;
    using Logging;

    public static class MailHelper
    {
        public static bool SendEmail(string from, string to, string subject, string body)
        {
            using (var smtpClient = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = AppSettings.SmtpClientEnableSsl,
                Host = AppSettings.SmtpClientHost,
                Port = AppSettings.SmtpClientPort,
                UseDefaultCredentials = false,
            })
            {
                smtpClient.Credentials = new NetworkCredential(AppSettings.SmtpClientAccount, AppSettings.SmtpClientPassword);

                var emailFrom = new MailAddress(from, from, Encoding.UTF8);
                var emailTo = new MailAddress(to, to, Encoding.UTF8);
                var message = new MailMessage(emailFrom, emailTo)
                {
                    SubjectEncoding = Encoding.UTF8,
                    Subject = subject,
                    BodyEncoding = Encoding.UTF8,
                    IsBodyHtml = true,
                    Body = body
                };

                try
                {
                    smtpClient.SendAsync(message, Guid.NewGuid());
                    return true;
                }
                catch (Exception exception)
                {
                    LogService.Error(exception.Message, exception);
                }

                return false;
            }
        }
    }
}