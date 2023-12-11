namespace OddsFeed.WebService.Criteria
{
    using Fanex.Data.Repository;

    public class GetCountryByUserNameCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey() => "Odds_CountrySelByUserName";

        public override bool IsValid() => !string.IsNullOrEmpty(Username);
    }
}