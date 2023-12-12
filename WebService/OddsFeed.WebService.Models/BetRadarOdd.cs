namespace OddsFeed.WebService.Models
{
    public class BetRadarOdd
    {
        public int MatchId { get; set; }
        public string LeagueName { get; set; }
        public string Home { get; set; }
        public string Away { get; set; }
        public string SportTypeName { get; set; }
        public string KickOffTime { get; set; }

        public short? OddsID_1 { get; set; }
        public float? Hdp1_1 { get; set; }
        public float? Hdp2_1 { get; set; }
        public decimal? Odds1hka_1 { get; set; }
        public decimal? Odds2hka_1 { get; set; }

        public short? OddsID_5 { get; set; }
        public decimal? Com1_5 { get; set; }
        public decimal? ComX_5 { get; set; }
        public decimal? Com2_5 { get; set; }

        public short? OddsID_3 { get; set; }
        public decimal? Hdp1_3 { get; set; }
        public decimal? Odds1hka_3 { get; set; }
        public decimal? Odds2hka_3 { get; set; }

        public short? OddsID_501 { get; set; }
        public decimal? Odds1hka_501 { get; set; }
        public decimal? Odds2hka_501 { get; set; }
    }
}