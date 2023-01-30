namespace Fanex.OddsFeed.Web.Services.Criteria.BetDaq
{
    using Fanex.Data.Repository;

    public class GetOddsFeedCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "SP.GetOddsFeed";

        public override bool IsValid() => true;
    }
}