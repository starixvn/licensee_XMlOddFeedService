namespace Fanex.OddsFeed.AutoJobs.Models.Matches
{
    using Newtonsoft.Json;

    [Serializable]
    public class MatchResultDetail
    {
        [JsonIgnore]
        public int MatchId { get; set; }

        public string ResourceId { get; set; }
        public string HomeValue { get; set; }
        public string AwayValue { get; set; }
        public byte ResultStatusId { get; set; }
        public byte SettlementStatusId { get; set; }
    }
}