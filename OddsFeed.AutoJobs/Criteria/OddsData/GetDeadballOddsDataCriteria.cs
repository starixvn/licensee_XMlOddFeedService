namespace OddsFeed.AutoJobs.Criteria.OddsData
{
    using Fanex.Data.Repository;

    public class GetDeadballOddsDataCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Is_Odds_GetDeadballOddsData";

        public override bool IsValid() => true;
    }
}