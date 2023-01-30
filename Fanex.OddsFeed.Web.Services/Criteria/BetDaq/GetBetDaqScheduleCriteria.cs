namespace Fanex.OddsFeed.Web.Services.Criteria.BetDaq
{
    using Fanex.Data.Repository;

    public class GetBetDaqScheduleCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Portal_BetDaq_Schedule";

        public override bool IsValid() => true;
    }
}