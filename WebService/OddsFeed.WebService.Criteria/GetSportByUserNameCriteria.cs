namespace OddsFeed.WebService.Criteria
{
    using Fanex.Data.Repository;

    public class GetSportByUserNameCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey() => "Odds_SportSelByUserName";

        public override bool IsValid() => !string.IsNullOrEmpty(Username);
    }
}