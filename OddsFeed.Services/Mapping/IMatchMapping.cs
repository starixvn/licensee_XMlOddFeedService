namespace OddsFeed.Services.Mapping
{
    using System.Collections.Generic;
    using OddsFeed.Models;
    using OddsFeed.Models.Matches;
    using OddsFeed.Models.OddsData;
    using OddsFeed.Models.Oddsfeed;

    public interface IMatchMapping
    {
        IList<Match> GetMatches(
            IEnumerable<MatchOddsData> odds,
            IEnumerable<BetTypeSchema> schemas,
            IEnumerable<OddsFlexGroup> oddsFlexs,
            bool isLive);

        Match CreateMatch(MatchOddsData row, bool isLive);
    }
}