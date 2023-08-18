namespace Fanex.OddsFeed.WebServiceLibrary.Criteria
{
    using Fanex.Data.Repository;

    public class GetSportByUserNameCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey() => "Odds_SportSelByUserName";
    }
}