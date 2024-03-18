namespace Fanex.OddsFeed.WebServiceLibrary.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class WriteLogRequestOddsCriteria : CriteriaBase
    {
        public string Username { get; set; }
        public string IP { get; set; }
        public string MatchIds { get; set; }
        public string TrackingValue { get; set; } = string.Empty;
        public int Duration { get; set; }
        public DateTime ExecutionTime { get; set; }

        public override string GetSettingKey() => "Odds_WriteLog_UserLog_RunSP";

        public override bool IsValid() => true;
    }
}