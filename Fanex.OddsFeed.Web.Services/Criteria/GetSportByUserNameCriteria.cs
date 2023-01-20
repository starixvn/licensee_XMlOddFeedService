namespace Fanex.OddsFeed.Web.Services.Criteria
{
    using Fanex.Data.Repository;

    public class GetSportByUserNameCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey()
        => "SP.GetSportByUserName";

        public override bool IsValid() => !string.IsNullOrEmpty(Username);
    }
}