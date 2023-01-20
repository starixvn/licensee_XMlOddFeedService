namespace Fanex.OddsFeed.Models.Matches
{
    [Serializable]
    public class Match
    {
        public int MatchId { get; set; }

        public int LeagueId { get; set; }
        public string LeagueEn { get; set; }
        public string LeagueTw { get; set; }
        public string LeagueCn { get; set; }
        public string LeagueIndonesia { get; set; }
        public string LeagueThai { get; set; }
        public string LeagueKorean { get; set; }
        public string LeagueVietnamese { get; set; }
        public string LeagueJapanese { get; set; }
        public string LeagueHindi { get; set; }

        public string HomeEn { get; set; }
        public string HomeTw { get; set; }
        public string HomeCn { get; set; }
        public string HomeIndonesia { get; set; }
        public string HomeThai { get; set; }
        public string HomeKorean { get; set; }
        public string HomeVietnamese { get; set; }
        public string HomeJapanese { get; set; }
        public string HomeHindi { get; set; }

        public string AwayEn { get; set; }
        public string AwayTw { get; set; }
        public string AwayCn { get; set; }
        public string AwayIndonesia { get; set; }
        public string AwayThai { get; set; }
        public string AwayKorean { get; set; }
        public string AwayVietnamese { get; set; }
        public string AwayJapanese { get; set; }
        public string AwayHindi { get; set; }

        public int SportId { get; set; }
        public string SportName { get; set; }

        public string CountryCode { get; set; }
        public DateTime? KickOffTime { get; set; }
        public DateTime? GlobalShowtime { get; set; }
        public DateTime? Showtime { get; set; }
        public string EventStatus { get; set; }

        public int Timestamp { get; set; }
        public bool IsLive { get; set; }
        public IList<MatchOdds> MatchOddsList { get; set; } = new List<MatchOdds>();
        public IList<MatchResultDetail> MatchResultDetails { get; set; } = new List<MatchResultDetail>();
    }
}