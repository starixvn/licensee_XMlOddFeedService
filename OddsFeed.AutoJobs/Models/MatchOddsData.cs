namespace OddsFeed.AutoJobs.Models
{
    using System;

    [Serializable]
    public class MatchOddsData
    {
        public int MatchID { get; set; }
        public DateTime? KickOffTime { get; set; }
        public DateTime? GlobalShowtime { get; set; }
        public DateTime? Showtime { get; set; }

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

        public float Hdp1 { get; set; }
        public float Hdp2 { get; set; }
        public float Odds1a { get; set; }
        public float Odds2a { get; set; }
        public float Com1 { get; set; }
        public float ComX { get; set; }
        public float Com2 { get; set; }

        public float? Cs00 { get; set; }
        public float? Cs11 { get; set; }
        public float? Cs22 { get; set; }
        public float? Cs33 { get; set; }
        public float? Cs44 { get; set; }
        public float? Cs01 { get; set; }
        public float? Cs02 { get; set; }
        public float? Cs03 { get; set; }
        public float? Cs04 { get; set; }
        public float? Cs05 { get; set; }
        public float? Cs10 { get; set; }
        public float? Cs20 { get; set; }
        public float? Cs30 { get; set; }
        public float? Cs40 { get; set; }
        public float? Cs50 { get; set; }
        public float? Cs12 { get; set; }
        public float? Cs13 { get; set; }
        public float? Cs23 { get; set; }
        public float? Cs14 { get; set; }
        public float? Cs24 { get; set; }
        public float? Cs34 { get; set; }
        public float? Cs21 { get; set; }
        public float? Cs31 { get; set; }
        public float? Cs32 { get; set; }
        public float? Cs41 { get; set; }
        public float? Cs42 { get; set; }
        public float? Cs43 { get; set; }

        public decimal? Cs51 { get; set; }
        public decimal? Cs52 { get; set; }
        public decimal? Cs53 { get; set; }
        public decimal? Cs54 { get; set; }

        public decimal? Cs15 { get; set; }
        public decimal? Cs25 { get; set; }
        public decimal? Cs35 { get; set; }
        public decimal? Cs45 { get; set; }

        public float? Cs99 { get; set; }

        public string ResourceID { get; set; }

        public int Timestamp { get; set; }
        public DateTime? ChangeTime { get; set; }
    }
}