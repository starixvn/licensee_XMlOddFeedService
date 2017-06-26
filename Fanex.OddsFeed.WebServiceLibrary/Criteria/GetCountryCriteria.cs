namespace Fanex.OddsFeed.WebServiceLibrary.Criteria
{
    using Common.Helpers;
    using Fanex.Data.Repository;

    public class GetCountryCriteria : CriteriaBase
    {
        public string Domain { get; set; }

        public string Ip { get; set; }

        public string Username { get; set; }

        public int? FromId { get; set; }

        public override string GetSettingKey()
        {
            return SpHelper.RealSp("SP.Ocs_Country");
        }
    }
}
