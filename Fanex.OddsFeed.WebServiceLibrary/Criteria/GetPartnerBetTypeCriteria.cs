namespace Fanex.OddsFeed.WebServiceLibrary.Criteria
{
    using Common.Helpers;
    using Fanex.Data.Repository;

    public class GetPartnerBetTypeCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey()
        {
            return SpHelper.RealSp("SP.BetType");
        }
    }
}
