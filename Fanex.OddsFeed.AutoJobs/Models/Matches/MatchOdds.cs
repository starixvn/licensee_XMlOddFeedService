namespace Fanex.OddsFeed.AutoJobs.Models.Matches
{
    using Newtonsoft.Json;

    public class MatchOdds
    {
        [JsonIgnore]
        public int MatchId { get; set; }

        public int BetTypeId { get; set; }

        public string Tag { get; set; }

        public int Timestamp { get; set; }

        public DateTime? Time { get; set; }

        public IList<BetChoice> BetChoices { get; set; }
    }
}