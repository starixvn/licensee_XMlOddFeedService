namespace OddsFeed.WebService.Criteria.BetDaq
{
    using Fanex.Data.Repository;

    public class GetOddsFeed2Criteria : CriteriaBase
    {
        public override string GetSettingKey() => "Portal_BetDaq_Odds_2_V2";

        public override bool IsValid() => true;
    }
}