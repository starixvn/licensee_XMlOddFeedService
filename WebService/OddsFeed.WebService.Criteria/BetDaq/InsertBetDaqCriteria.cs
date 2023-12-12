namespace OddsFeed.WebService.Criteria.BetDaq
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertBetDaqCriteria : CriteriaBase
    {
        public InsertBetDaqCriteria(string settingKey)
        {
            SettingKey = !string.IsNullOrWhiteSpace(settingKey) ? settingKey : "Portal_BetDaq_InsertBet";
        }

        public int OddsId { get; set; }
        public decimal hdp1 { get; set; }
        public decimal hdp2 { get; set; }
        public string betteam { get; set; }
        public decimal odds { get; set; }
        public decimal stake { get; set; }
        public int betdaqid { get; set; }
        public string bet_ip { get; set; }

        [SpParam(Ignored = true)]
        public string SettingKey { get; private set; }

        public override string GetSettingKey() => SettingKey;

        public override bool IsValid() => true;
    }
}