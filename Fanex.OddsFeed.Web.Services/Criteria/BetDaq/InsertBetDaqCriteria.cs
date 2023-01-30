namespace Fanex.OddsFeed.Web.Services.Criteria.BetDaq
{
    using Fanex.Data.Repository;

    public class InsertBetDaqCriteria : CriteriaBase
    {
        public InsertBetDaqCriteria(string settingKey)
        {
            SettingKey = !string.IsNullOrWhiteSpace(settingKey) ? settingKey : "SP.InsertBetDaq";
        }

        public string SettingKey { get; private set; }

        public override string GetSettingKey() => SettingKey;

        public override bool IsValid() => true;
    }
}