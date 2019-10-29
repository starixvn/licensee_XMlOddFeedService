namespace Fanex.OddsFeed.WebServiceLibrary.Criteria
{
    using Common.Helpers;
    using Fanex.Data.Repository;

    public class WriteLogRequestAccessCriteria : CriteriaBase
    {
        public string UserName { get; set; }

        public string DomainName { get; set; }

        public string Ip { get; set; }

        public override string GetSettingKey()
        {
            return SpHelper.RealSp("Odds_PartnerCheckPermission_WriteLog");
        }
    }
}
