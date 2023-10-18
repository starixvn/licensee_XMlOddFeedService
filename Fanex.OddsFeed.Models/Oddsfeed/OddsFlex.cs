namespace Fanex.OddsFeed.Models.Oddsfeed
{
    [Serializable]
    public class OddsFlex
    {
        public int OddId { get; set; }
        public int BetType { get; set; }
        public string BetChoice { get; set; }
        public decimal Odds { get; set; }
        public string OddsInfo { get; set; }
    }
}