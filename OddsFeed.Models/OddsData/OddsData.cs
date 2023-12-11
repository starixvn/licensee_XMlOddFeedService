namespace OddsFeed.Models.OddsData
{
    using System.Collections.Generic;
    using OddsFeed.Models.Matches;
    using OddsFeed.Models.Outright;

    public class OddsData
    {
        public IList<Match> LiveMatches { get; set; }
        public IList<Match> DeadballMatches { get; set; }
        public IList<Outright> Outrights { get; set; }
    }
}