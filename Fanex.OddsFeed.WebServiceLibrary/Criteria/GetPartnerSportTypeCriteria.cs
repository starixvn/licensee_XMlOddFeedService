namespace Fanex.OddsFeed.WebServiceLibrary.Criteria
{
    using Fanex.Data.Repository;

    public class GetPartnerSportTypeCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey() => "Odds_Partner_SportTypeSel";
    }
}