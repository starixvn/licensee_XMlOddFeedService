namespace OddsFeed.WebService.Criteria
{
    using Fanex.Data.Repository;

    public class GetPartnerBetTypeCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey()
          => "Odds_Partner_BetTypeSel";

        public override bool IsValid() => !string.IsNullOrEmpty(Username);
    }
}