namespace Fanex.OddsFeed.Web.Services.Criteria
{
    using Fanex.Data.Repository;

    public class CheckPermissionCriteria : CriteriaBase
    {
        public string UserName { get; set; }
        public string DomainName { get; set; }
        public string Ip { get; set; }
        public string SystemName { get; set; }
        public int ErrCode { get; set; } = 0;
        public string ErrMsg { get; set; } = string.Empty;

        public override string GetSettingKey() => "Odds_PartnerCheckPermission";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(UserName)
            && !string.IsNullOrWhiteSpace(DomainName)
            && !string.IsNullOrWhiteSpace(Ip)
            && !string.IsNullOrWhiteSpace(SystemName);
    }
}