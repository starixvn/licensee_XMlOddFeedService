namespace OddsFeed.AutoJobs.Criteria.OddsData
{
    using Fanex.Data.Repository;

    public class GetOutrightOddsDataCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Is_Odds_GetOutrightOddsData";

        public override bool IsValid() => true;
    }
}