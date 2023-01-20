namespace Fanex.OddsFeed.Models.OddsData
{
    using Fanex.OddsFeed.Models.Matches;
    using Fanex.OddsFeed.Models.Outright;

    public class OddsData
    {
        public IList<Match> LiveMatches { get; set; }
        public IList<Match> DeadballMatches { get; set; }
        public IList<Outright> Outrights { get; set; }
    }
}