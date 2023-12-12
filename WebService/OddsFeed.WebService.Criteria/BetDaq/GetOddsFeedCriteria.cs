namespace OddsFeed.WebService.Criteria.BetDaq
{
    using Fanex.Data.Repository;

    public class GetOddsFeedCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Portal_BetDaq_Odds_V2";

        public override bool IsValid() => true;
    }
}