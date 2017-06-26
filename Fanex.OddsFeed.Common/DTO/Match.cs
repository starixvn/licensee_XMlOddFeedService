namespace Fanex.OddsFeed.Common.DTO
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    [Serializable]
    public class LeagueDocument
    {
        public int Version { get; set; }

        public int LeagueId { get; set; }

        public string LeagueEn { get; set; }

        public string LeagueTw { get; set; }

        public string LeagueCn { get; set; }

        public int SportId { get; set; }

        public string SportName { get; set; }

        public string CountryCode { get; set; }

        public IList<Match> Matches { get; set; }
    }

    [Serializable]
    public class MatchToMap
    {
        public int MatchId { get; set; }

        public int LeagueId { get; set; }

        [JsonProperty("LeagueEnglish")]
        public string LeagueEn { get; set; }

        [JsonProperty("LeagueTraditionalChinese")]
        public string LeagueTw { get; set; }

        [JsonProperty("LeagueSimplifiedChinese")]
        public string LeagueCn { get; set; }

        [JsonProperty("HomeEnglish")]
        public string HomeEn { get; set; }

        [JsonProperty("HomeTraditionalChinese")]
        public string HomeTw { get; set; }

        [JsonProperty("HomeSimplifiedChinese")]
        public string HomeCn { get; set; }

        [JsonProperty("AwayEnglish")]
        public string AwayEn { get; set; }

        [JsonProperty("AwayTraditionalChinese")]
        public string AwayTw { get; set; }

        [JsonProperty("AwaySimplifiedChinese")]
        public string AwayCn { get; set; }

        public int SportId { get; set; }

        public string SportName { get; set; }

        public string CountryCode { get; set; }

        public DateTime KickOfTime { get; set; }

        public int TimeStamp { get; set; }

        public IList<MatchOdds> MatchOddsList { get; set; }
    }

    [Serializable]
    public class Match
    {
        public int MatchId { get; set; }

        public int LeagueId { get; set; }

        public string LeagueEn { get; set; }

        public string LeagueTw { get; set; }

        public string LeagueCn { get; set; }

        public string HomeEn { get; set; }

        public string HomeTw { get; set; }

        public string HomeCn { get; set; }

        public string AwayEn { get; set; }

        public string AwayTw { get; set; }

        public string AwayCn { get; set; }

        public int SportId { get; set; }

        public string SportName { get; set; }

        public string CountryCode { get; set; }

        public DateTime KickOfTime { get; set; }

        public int TimeStamp { get; set; }

        public bool IsLive { get; set; }

        public IList<MatchOdds> MatchOddsList { get; set; }
    }
}
