namespace OddsFeed.WebService.Criteria
{
    using Fanex.Data.Repository;

    public class WriteLogRequestAccessCriteria : CriteriaBase, INonQueryCommand
    {
        public string UserName { get; set; }

        public string DomainName { get; set; }

        public string Ip { get; set; }

        public override string GetSettingKey() => "Odds_PartnerCheckPermission_WriteLog";

        public override bool IsValid() => true;
    }
}