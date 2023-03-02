namespace Fanex.OddsFeed.Core.Models.Email
{
    public class EmailModel
    {
        public string SenderEmail { get; set; }
        public string ReceiverEmail { get; set; }
        public string MailCC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}