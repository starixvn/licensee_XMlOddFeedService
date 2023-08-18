namespace Fanex.OddsFeed.Common.Criteria
{
    using Fanex.Data.Repository;

    public class GetLiveOddsDataCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Is_Odds_GetLiveOddsData";

        public override bool IsValid() => true;
    }
}