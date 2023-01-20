namespace Fanex.OddsFeed.Web.Services.Criteria
{
    using Fanex.Data.Repository;

    public class GetCountryByUserNameCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey() => "SP.GetCountryByUserName";

        public override bool IsValid() => !string.IsNullOrEmpty(Username);
    }
}