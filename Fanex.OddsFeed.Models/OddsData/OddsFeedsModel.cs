namespace Fanex.OddsFeed.Models.OddsData
{
    using Fanex.OddsFeed.Models.DTO;

    public class OddsFeedsModel
    {
        public UserProfile UserProfile { get; set; }

        public UserQuery UserQuery { get; set; }

        public string BookieName { get; set; }

        public int Timestamp { get; set; }

        public bool IsSample { get; set; }

        public bool IsOutright { get; set; } = true;

        public string Username { get; set; }

        public DateTime? Date { get; set; }
    }
}