namespace Fanex.OddsFeed.WebServiceLibrary.Criteria
{
    using Fanex.Data.Repository;

    public class GetPartnerBetTypeCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey() => "Odds_Partner_BetTypeSel";
    }
}