namespace OddsFeed.WebService.Criteria
{
    using Fanex.Data.Repository;

    public class GetBetRadarCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "SP.BetRadarOddsSel";

        public override bool IsValid() => true;
    }
}