namespace Fanex.OddsFeed.WebServiceLibrary.Models
{
    using Fanex.OddsFeed.Common.DTO;

    public class OddsFeedsModel
    {
        public UserProfile UserProfile { get; set; }

        public UserQuery UserQuery { get; set; }

        public string BookieName { get; set; }

        public string DomainName { get; set; }

        public string IpAddress { get; set; }

        public int TimeStamp { get; set; }

        public bool IsSample { get; set; }

        public bool IsOutright { get; set; } = true;

        public string Username { get; set; }
    }
}
