namespace OddsFeed.WebService.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetMatchResultDetailCriteria : CriteriaBase
    {
        public DateTime? EventDate { get; set; }

        public override string GetSettingKey() => "Odds_CricketResult_Sel";

        public override bool IsValid() => true;
    }
}