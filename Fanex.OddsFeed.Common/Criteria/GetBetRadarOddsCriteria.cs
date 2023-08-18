namespace Fanex.OddsFeed.Common.Criteria
{
    using Fanex.Data.Repository;

    public class GetBetRadarOddsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Odds_BetRadar_OddsSel";

        public override bool IsValid() => true;
    }
}