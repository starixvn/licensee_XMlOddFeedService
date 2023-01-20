namespace Fanex.OddsFeed.Web.Services.Criteria
{
    using Fanex.Data.Repository;

    public class WriteLogRequestAccessCriteria : CriteriaBase
    {
        public string UserName { get; set; }

        public string DomainName { get; set; }

        public string Ip { get; set; }

        public override string GetSettingKey() => "Odds_PartnerCheckPermission_WriteLog";

        public override bool IsValid() => true;
    }
}