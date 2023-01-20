namespace Fanex.OddsFeed.Web.Services.Criteria
{
    using Fanex.Data.Repository;

    public class GetBetTypeByUserNameCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey()
            => "SP.GetBetTypeByUserName";

        public override bool IsValid() => !string.IsNullOrEmpty(Username);
    }
}