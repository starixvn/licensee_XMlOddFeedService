namespace OddsFeed.Models.Oddsfeed
{
    using System;

    [Serializable]
    public class OddsFlex
    {
        public int OddsId { get; set; }
        public int BetType { get; set; }
        public string BetChoice { get; set; }
        public decimal Odds { get; set; }
        public string OddsInfo { get; set; }
    }
}