namespace OddsFeed.AutoJobs.Mapping
{
    using System.Collections.Generic;
    using OddsFeed.AutoJobs.Models;
    using OddsFeed.Models;
    using OddsFeed.Models.Matches;
    using OddsFeed.Models.Oddsfeed;

    public interface IMatchMapping
    {
        IList<Match> GetMatches(
            IEnumerable<MatchOddsData> odds,
            IEnumerable<BetTypeSchema> schemas,
            IEnumerable<OddsFlexGroup> oddsFlexs,
            bool isLive);
    }
}