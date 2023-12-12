namespace OddsFeed.WebService.Criteria
{
    using Fanex.Data.Repository;

    public class GetBetRadarCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Odds_BetRadar_OddsSel";

        public override bool IsValid() => true;
    }
}