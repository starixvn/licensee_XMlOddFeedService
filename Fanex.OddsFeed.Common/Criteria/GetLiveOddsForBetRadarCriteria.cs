namespace Fanex.OddsFeed.Common.Criteria
{
    using Fanex.Data.Repository;

    public class GetLiveOddsForBetRadarCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Is_Odds_GetLiveOddsData_Betradar";

        public override bool IsValid() => true;
    }
}