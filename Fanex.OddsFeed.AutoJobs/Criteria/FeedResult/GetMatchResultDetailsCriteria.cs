namespace Fanex.OddsFeed.AutoJobs.Criteria.FeedResult
{
    using Fanex.Data.Repository;

    public class GetMatchResultDetailsCriteria : CriteriaBase
    {
        public DateTime EventDate { get; set; }

        public override string GetSettingKey() => "Odds_CricketResult_Sel";

        public override bool IsValid() => true;
    }
}