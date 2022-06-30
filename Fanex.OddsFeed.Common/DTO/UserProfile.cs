namespace Fanex.OddsFeed.Common.DTO
{
    using global::System.Collections.Generic;

    public class UserProfile
    {
        public IList<int> OddsType { get; set; }

        public int OddRegion { get; set; }

        public IList<string> Languages { get; set; }

        public IList<int> Sports { get; set; }

        public IList<string> Countries { get; set; }
   
        public IList<int> Leagues { get; set; }

        public IList<int> Outrights { get; set; }

        public IList<int> BetTypes { get; set; }

        public IList<int> OddsDelayTimes { get; set; }

        public string LeagueKeyword { get; set; }

        public string OutrightsKeyword { get; set; }

        public int TimeRange { get; set; }

        public string SpParams { get; set; }

        public string Domain { get; set; }

        public int ResultDelayTimes { get; set; }
    }
}
