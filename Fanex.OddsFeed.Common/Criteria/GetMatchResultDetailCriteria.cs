namespace Fanex.OddsFeed.Common.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetMatchResultDetailCriteria : CriteriaBase
    {
        public DateTime Date { get; set; }
        public override string GetSettingKey() => "";

        public override bool IsValid() => true;
    }
}