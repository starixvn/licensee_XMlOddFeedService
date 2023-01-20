namespace Fanex.OddsFeed.Web.Services.Criteria
{
    using Fanex.Data.Repository;

    public class GetLeagueCriteria : CriteriaBase
    {
        public string Domain { get; set; }

        public string Ip { get; set; }

        public string Username { get; set; }

        public int? FromId { get; set; }

        public override string GetSettingKey()
       => "SP.Ocs_League";

        public override bool IsValid() => true;
    }
}