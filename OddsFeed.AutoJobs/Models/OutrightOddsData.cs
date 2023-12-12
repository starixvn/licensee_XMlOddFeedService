namespace OddsFeed.AutoJobs.Models
{
    using System;

    [Serializable]
    public class OutrightOddsData
    {
        public int OrID { get; set; }
        public int LeagueID { get; set; }
        public short SportID { get; set; }
        public decimal Odds { get; set; }
        public DateTime EventDate { get; set; }

        public string LeagueName { get; set; }
        public string LeagueNameC_UNI { get; set; }
        public string LeagueNameCN { get; set; }
        public string LeagueNameId { get; set; }
        public string LeagueNameTh { get; set; }
        public string LeagueNameKo { get; set; }
        public string LeagueNameVn { get; set; }
        public string LeagueNameJp { get; set; }
        public string LeagueNameHi { get; set; }
        public string LeagueNameTC { get; set; }

        public string TeamName { get; set; }
        public string TeamNameC_UNI { get; set; }
        public string TeamNameCN { get; set; }
        public string TeamNameId { get; set; }
        public string TeamNameTh { get; set; }
        public string TeamNameKo { get; set; }
        public string TeamNameVn { get; set; }
        public string TeamNameJp { get; set; }
        public string TeamNameHi { get; set; }

        public string CountryCode { get; set; }
    }
}