namespace OddsFeed.AutoJobs.Models
{
    using System;

    [Serializable]
    public class MatchOddsData
    {
        public int MatchID { get; set; }
        public DateTime? KickOffTime { get; set; }
        public DateTime? GlobalShowtime { get; set; }
        public DateTime? ShowTime { get; set; }

        public short SportType { get; set; }
        public string SportTypeName { get; set; }

        public string Home { get; set; }
        public string HomeCN { get; set; }
        public string HomeC_UNI { get; set; }
        public string HomeId { get; set; }
        public string HomeTh { get; set; }
        public string HomeKo { get; set; }
        public string HomeVn { get; set; }
        public string HomeJp { get; set; }
        public string HomeHi { get; set; }

        public string Away { get; set; }
        public string AwayCN { get; set; }
        public string AwayC_UNI { get; set; }
        public string AwayId { get; set; }
        public string AwayTh { get; set; }
        public string AwayKo { get; set; }
        public string AwayVn { get; set; }
        public string AwayJp { get; set; }
        public string AwayHi { get; set; }

        public int LeagueID { get; set; }
        public string LeagueName { get; set; }
        public string LeagueNameCN { get; set; }
        public string LeagueNameC_UNI { get; set; }
        public string LeagueNameId { get; set; }
        public string LeagueNameTh { get; set; }
        public string LeagueNameKo { get; set; }
        public string LeagueNameVn { get; set; }
        public string LeagueNameJp { get; set; }
        public string LeagueNameHi { get; set; }

        public string CountryCode { get; set; }

        public short BetType { get; set; }
        public int OddsID { get; set; }
        public string ResourceID { get; set; }

        public int Timestamp { get; set; }
        public DateTime? ChangeTime { get; set; }
    }
}