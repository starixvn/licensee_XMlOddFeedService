namespace Fanex.OddsFeed.Models.Data
{
    public class ProfileData
    {
        public int PartnerId { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime LastUpdate { get; set; }
        public string OddsType { get; set; }
        public string LanguageList { get; set; }
        public byte OddsRegion { get; set; }
        public string SportIDList { get; set; }
        public string BetTypeIDList { get; set; }
        public string LeagueIDList { get; set; }
        public string CountryCodeList { get; set; }
        public string LeagueKeyword { get; set; }
        public string LeagueKeyword_O { get; set; }
        public string OddsDelayTimes { get; set; }
        public string OutrightIDList { get; set; }
        public short NumberOfIPs { get; set; }
        public string Domain { get; set; }
        public string ResultDelayTimes { get; set; }
    }
}