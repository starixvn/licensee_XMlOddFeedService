namespace OddsFeed.WebService.Criteria.BetDaq
{
    using Fanex.Data.Repository;

    public class GetOddsFeed2Criteria : CriteriaBase
    {
        public override string GetSettingKey() => "SP.GetOddsFeed_2";

        public override bool IsValid() => true;
    }
}