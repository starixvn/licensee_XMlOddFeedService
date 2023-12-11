namespace OddsFeed.WebService.Criteria
{
    using Fanex.Data.Repository;

    public class GetContactEmailByUserNameCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey() => "Odds_ContactEmailSelByUserName";

        public override bool IsValid() => !string.IsNullOrEmpty(Username);
    }
}