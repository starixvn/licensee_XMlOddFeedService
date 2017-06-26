namespace Fanex.OddsFeed.Common.DTO
{
    using System;
    using System.Collections.Generic;

    public class MatchNode
    {
        public int MatchId { get; set; }

        public string LeagueEn { get; set; }

        public string LeagueCn { get; set; }

        public string LeagueTw { get; set; }

        public string HomeEn { get; set; }

        public string HomeCn { get; set; }

        public string HomeTw { get; set; }

        public string AwayEn { get; set; }

        public string AwayCn { get; set; }

        public string AwayTw { get; set; }

        public string SportName { get; set; }

        public DateTime KickOfTime { get; set; }

        public string Tag { get; set; }

        public DateTime Time { get; set; }

        public int TimeStamp { get; set; }

        public IList<BetChoice> BetChoices { get; set; }
    }
}
