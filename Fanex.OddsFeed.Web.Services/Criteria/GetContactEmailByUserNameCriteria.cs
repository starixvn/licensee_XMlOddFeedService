namespace Fanex.OddsFeed.Web.Services.Criteria
{
    using Fanex.Data.Repository;

    public class GetContactEmailByUserNameCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey() => "SP.GetContactEmailByUserName";

        public override bool IsValid() => !string.IsNullOrEmpty(Username);
    }
}