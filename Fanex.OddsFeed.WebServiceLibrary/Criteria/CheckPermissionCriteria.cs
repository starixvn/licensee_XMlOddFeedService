namespace Fanex.OddsFeed.WebServiceLibrary.Criteria
{
    using Common.Helpers;
    using Fanex.Data.Repository;

    public class CheckPermissionCriteria : CriteriaBase
    {
        public string UserName { get; set; }

        public string DomainName { get; set; }

        public string Ip { get; set; }

        public string SystemName { get; set; }

        public override string GetSettingKey()
        {
            return SpHelper.RealSp("SP.CheckPermission");
        }
    }
}
