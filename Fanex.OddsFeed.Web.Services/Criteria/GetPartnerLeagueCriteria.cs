namespace Fanex.OddsFeed.Web.Services.Criteria
{
    using Fanex.Data.Repository;

    public class GetPartnerLeagueCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey()
        => "Odds_Partner_LeagueSel";

        public override bool IsValid() => !string.IsNullOrEmpty(Username);
    }
}