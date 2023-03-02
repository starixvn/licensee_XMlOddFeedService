namespace Fanex.OddsFeed.Core.Models.Email
{
    public class EmailSettings
    {
        public bool IsDisabledSendMail { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}