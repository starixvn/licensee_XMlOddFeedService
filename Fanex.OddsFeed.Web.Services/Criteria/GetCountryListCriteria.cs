namespace Fanex.OddsFeed.Web.Services.Criteria
{
    using Fanex.Data.Repository;

    public class GetCountryListCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "SP.GetCountryList";

        public override bool IsValid() => true;
    }
}