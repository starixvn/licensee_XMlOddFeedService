namespace Fanex.OddsFeed.WebServiceLibrary.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.OddsFeed.Common.Helpers;

    public class GetPartnerLeagueCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey()
        {
            return SpHelper.RealSp("SP.League");
        }
    }
}
