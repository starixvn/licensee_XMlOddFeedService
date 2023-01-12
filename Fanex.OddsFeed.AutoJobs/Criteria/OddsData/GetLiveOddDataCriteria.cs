namespace Fanex.OddsFeed.AutoJobs.Criteria.OddsData
{
    using Fanex.Data.Repository;

    public class GetLiveOddDataCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Is_Odds_GetLiveOddsData";

        public override bool IsValid() => true;
    }
}