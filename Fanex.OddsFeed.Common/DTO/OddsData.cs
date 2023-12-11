namespace Fanex.OddsFeed.Common.DTO
{
    using System.Collections.Generic;

    public class OddsData
    {
        public IList<Match> LiveMatches { get; set; }

        public IList<Match> DeadballMatches { get; set; }

        public IList<Outright> Outrights { get; set; }
    }
}
